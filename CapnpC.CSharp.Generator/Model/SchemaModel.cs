using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static CapnpC.CSharp.Generator.Model.SupportedAnnotations;

namespace CapnpC.CSharp.Generator.Model
{
    class SchemaModel
    {
        readonly Schema.CodeGeneratorRequest.Reader _request;
        readonly List<GenFile> _generatedFiles = new List<GenFile>();
        readonly DefinitionManager _typeDefMgr = new DefinitionManager();

        readonly Dictionary<ulong, Schema.Node.Reader> _id2node = new Dictionary<ulong, Schema.Node.Reader>();

        public SchemaModel(Schema.CodeGeneratorRequest.Reader request)
        {
            _request = request;
        }

        public IReadOnlyList<GenFile> FilesToGenerate => _generatedFiles;

        Schema.Node.Reader? IdToNode(ulong id, bool mustExist)
        {
            if (_id2node.TryGetValue(id, out var node))
                return node;
            if (mustExist)
                throw new InvalidSchemaException($"Node with ID {id.StrId()} is required by the codegen backend but is missing.");
            return null;
        }

        Schema.Node.Reader IdToNode(ulong id)
        {
            return (Schema.Node.Reader)IdToNode(id, true);
        }

        void Build()
        {
            if (_request.Nodes == null || _request.Nodes.Count == 0)
            {
                throw new InvalidSchemaException("No nodes, nothing to generate");
            }

            foreach (var node in _request.Nodes)
            {
                if (_id2node.TryGetValue(node.Id, out var existingNode))
                {
                    throw new InvalidSchemaException($"Node {node.StrId()} \"{node.DisplayName}\" has a duplicate ID, prior node was \"{existingNode.DisplayName}\"");
                }
                _id2node[node.Id] = node;
            }

            var requestedFiles = _request.RequestedFiles.ToDictionary(req => req.Id);
            BuildPass1(requestedFiles);
            BuildPass2(requestedFiles);
        }

        // First pass: create type definitions for each node.

        struct Pass1State
        {
            public HashSet<ulong> unprocessedNodes;
            public IHasNestedDefinitions parent;
        }

        void BuildPass1(Dictionary<ulong, Schema.CodeGeneratorRequest.RequestedFile.Reader> requestedFiles)
        {
            Pass1State state = new Pass1State()
            {
                unprocessedNodes = new HashSet<ulong>(_id2node.Keys)
            };
            foreach (var node in _id2node.Values.Where(n => n.IsFile))
            {
                GenFile file;
                bool isGenerated = requestedFiles.TryGetValue(node.Id, out var req);
                var filename = isGenerated ? req.Filename : node.DisplayName;
                file = ProcessFilePass1(node.Id, filename, state, isGenerated);
                if (isGenerated)
                    _generatedFiles.Add(file);
            }
            if (state.unprocessedNodes.Count != 0)
            {
                throw new InvalidSchemaException("Unreferenced nodes were present in the schema.");
            }
        }

        GenFile ProcessFilePass1(ulong id, string name, Pass1State state, bool isGenerated)
        {
            var file = _typeDefMgr.CreateFile(id, isGenerated);
            var node = IdToNode(id);
            state.parent = null;
            file.Namespace = GetNamespaceAnnotation(node);
            file.Name = name;
            file.NullableEnable = GetNullableEnable(node);
            file.EmitNullableDirective = GetEmitNullableDirective(node) ?? false;
            return ProcessNodePass1(id, name, state) as GenFile;
        }

        IDefinition ProcessNodePass1(ulong id, string name, Pass1State state)
        {
            bool mustExist = state.parent == null || (state.parent as IDefinition).IsGenerated;
            if (!(IdToNode(id, mustExist) is Schema.Node.Reader node))
                return null;
            if (!state.unprocessedNodes.Remove(id))
                return null;

            IDefinition def = null;
            bool processNestedNodes = false;
            bool processFields = false;
            bool processInterfaceMethods = false;

            switch (node.GetKind())
            {
                case NodeKind.Annotation:
                    return _typeDefMgr.CreateAnnotation(id, state.parent);
                case NodeKind.Const:
                    return _typeDefMgr.CreateConstant(id, state.parent);
                case NodeKind.File:
                    if (state.parent != null)
                        throw new InvalidSchemaException($"Did not expect a file node {node.StrId()} to be a nested node.");
                    var file = _typeDefMgr.GetExistingFile(id);
                    file.Namespace = GetNamespaceAnnotation(node);
                    file.Name = name;
                    state.parent = file;
                    def = file;
                    processNestedNodes = true;
                    break;
                case NodeKind.Enum:
                    break;
                case NodeKind.Interface:
                    processNestedNodes = true;
                    processFields = true;
                    processInterfaceMethods = true;
                    break;
                case NodeKind.Struct:
                case NodeKind.Group:
                    processNestedNodes = true;
                    processFields = true;
                    break;
                default:
                    throw new InvalidSchemaException($"Don't know how to process node {node.StrId()} \"{node.DisplayName}\"");
            }

            if (def == null)
            {
                Trace.Assert(state.parent != null, $"The {node.GetTypeTag().ToString()} node {node.StrId()} was expected to have a parent.");
                var typeDef = _typeDefMgr.CreateTypeDef(id, node.GetTypeTag(), state.parent);
                typeDef.Name = name;
                typeDef.CsName = GetCsName(node);
                state.parent = typeDef;
                def = typeDef;
            }
            
            if (processNestedNodes && node.NestedNodes != null)
                foreach (var nested in node.NestedNodes)
                {
                    ProcessNodePass1(nested.Id, nested.Name, state);
                }
            if (processFields && node.Fields != null)
                foreach (var field in node.Fields.Where(f => f.IsGroup))
                {
                    var group = IdToNode(field.Group_TypeId);
                    if (!group.IsStruct || !group.Struct_IsGroup)
                    {
                        throw new InvalidSchemaException($"Expected node with id {group.StrId()} to be a struct definition");
                    }
                    ProcessNodePass1(field.Group_TypeId, field.Name, state);
                }
            if (processInterfaceMethods && node.Interface_Methods != null)
                foreach (var method in node.Interface_Methods)
                {
                    var pnode = IdToNode(method.ParamStructType);
                    if (pnode.ScopeId == 0) ProcessNodePass1(pnode.Id, null, state); // Anonymous generated type
                    pnode = IdToNode(method.ResultStructType);
                    if (pnode.ScopeId == 0) ProcessNodePass1(pnode.Id, null, state); // Anonymous generated type
                }
            return def;
        }

        // 2nd pass: Generate types based on definitions

        struct Pass2State
        {
            public Method currentMethod;
            public HashSet<ulong> processedNodes;
        }

        void BuildPass2(Dictionary<ulong, Schema.CodeGeneratorRequest.RequestedFile.Reader> requestedFiles)
        {
            var state = new Pass2State() { processedNodes = new HashSet<ulong>() };
            foreach (var file in _typeDefMgr.Files)
            {
                var node = IdToNode(file.Id);
                ProcessNestedNodes(node.NestedNodes, state, file.IsGenerated);
            }
        }

        void ProcessNestedNodes(IEnumerable<Schema.Node.NestedNode.Reader> nestedNodes, Pass2State state, bool mustExist)
        {
            foreach (var nestedNode in nestedNodes)
            {
                ProcessNode(nestedNode.Id, state, mustExist);
            }
        }

        void ProcessBrand(Schema.Brand.Reader brandReader, Type type, Pass2State state)
        {
            foreach (var scopeReader in brandReader.Scopes)
            {
                var whatToBind = ProcessTypeDef(scopeReader.ScopeId, state);
                int index = 0;

                switch (0)
                {
                    case 0 when scopeReader.IsBind:
                        foreach (var bindingReader in scopeReader.Bind)
                        {
                            var typeParameter = new GenericParameter()
                            {
                                DeclaringEntity = whatToBind,
                                Index = index++
                            };

                            switch (0)
                            {
                                case 0 when bindingReader.IsType:
                                    type.BindGenericParameter(typeParameter, ProcessType(bindingReader.Type, state));
                                    break;

                                case 0 when bindingReader.IsUnbound:
                                    type.BindGenericParameter(typeParameter, Types.FromParameter(typeParameter));
                                    break;
                            }
                        }
                        break;

                    case 0 when scopeReader.IsInherit:
                        for (index = 0; index < type.DeclaringType.Definition.GenericParameters.Count; index++)
                        {
                            var typeParameter = new GenericParameter()
                            {
                                DeclaringEntity = whatToBind,
                                Index = index
                            };

                            type.BindGenericParameter(typeParameter, Types.FromParameter(typeParameter));
                        }
                        break;
                }
            }
        }

        Type ProcessType(Schema.Type.Reader typeReader, Pass2State state)
        {
            Type result;

            switch (0)
            {
                case 0 when typeReader.IsAnyPointer:
                    switch (0)
                    {
                        case 0 when typeReader.AnyPointer_IsParameter:
                            return Types.FromParameter(
                                new GenericParameter()
                                {
                                    DeclaringEntity = ProcessTypeDef(typeReader.AnyPointer_Parameter_ScopeId, state),
                                    Index = typeReader.AnyPointer_Parameter_ParameterIndex
                                });

                        case 0 when typeReader.AnyPointer_IsImplicitMethodParameter:
                            return Types.FromParameter(
                                new GenericParameter()
                                {
                                    DeclaringEntity = state.currentMethod ?? throw new InvalidOperationException("current method not set"),
                                    Index = typeReader.AnyPointer_ImplicitMethodParameter_ParameterIndex
                                });

                        case 0 when typeReader.AnyPointer_IsUnconstrained:

                            switch (0)
                            {
                                case 0 when typeReader.AnyPointer_Unconstrained_IsAnyKind:
                                    return Types.AnyPointer;

                                case 0 when typeReader.AnyPointer_Unconstrained_IsCapability:
                                    return Types.CapabilityPointer;

                                case 0 when typeReader.AnyPointer_Unconstrained_IsList:
                                    return Types.ListPointer;

                                case 0 when typeReader.AnyPointer_Unconstrained_IsStruct:
                                    return Types.StructPointer;

                                default:
                                    throw new NotImplementedException();
                            }

                        default:
                            throw new NotImplementedException();
                    }

                case 0 when typeReader.IsBool:
                    return Types.Bool;

                case 0 when typeReader.IsData:
                    return Types.Data;

                case 0 when typeReader.IsFloat64:
                    return Types.F64;

                case 0 when typeReader.IsEnum:
                    return Types.FromDefinition(ProcessTypeDef(typeReader.Enum_TypeId, state, TypeTag.Enum));

                case 0 when typeReader.IsFloat32:
                    return Types.F32;

                case 0 when typeReader.IsInt16:
                    return Types.S16;

                case 0 when typeReader.IsInt32:
                    return Types.S32;

                case 0 when typeReader.IsInt64:
                    return Types.S64;

                case 0 when typeReader.IsInt8:
                    return Types.S8;

                case 0 when typeReader.IsInterface:
                    result = Types.FromDefinition(ProcessTypeDef(typeReader.Interface_TypeId, state, TypeTag.Interface));
                    ProcessBrand(typeReader.Interface_Brand, result, state);
                    return result;

                case 0 when typeReader.IsList:
                    return Types.List(ProcessType(typeReader.List_ElementType, state));

                case 0 when typeReader.IsStruct:
                    result = Types.FromDefinition(ProcessTypeDef(typeReader.Struct_TypeId, state, TypeTag.Struct));
                    ProcessBrand(typeReader.Struct_Brand, result, state);
                    return result;

                case 0 when typeReader.IsText:
                    return Types.Text;

                case 0 when typeReader.IsUInt16:
                    return Types.U16;

                case 0 when typeReader.IsUInt32:
                    return Types.U32;

                case 0 when typeReader.IsUInt64:
                    return Types.U64;

                case 0 when typeReader.IsUInt8:
                    return Types.U8;

                case 0 when typeReader.IsVoid:
                    return Types.Void;

                default:
                    throw new NotImplementedException();
            }
        }

        Value ProcessValue(Schema.Value.Reader valueReader)
        {
            var value = new Value();

            switch (0)
            {
                case 0 when valueReader.IsAnyPointer:
                    value.ScalarValue = valueReader.AnyPointer;
                    value.Type = Types.AnyPointer;
                    break;

                case 0 when valueReader.IsBool:
                    value.ScalarValue = valueReader.Bool;
                    value.Type = Types.Bool;
                    break;

                case 0 when valueReader.IsData:
                    value.Items.AddRange(valueReader.Data.CastByte().Select(Value.Scalar));
                    value.Type = Types.Data;
                    break;

                case 0 when valueReader.IsEnum:
                    value.ScalarValue = valueReader.Enum;
                    value.Type = Types.AnyEnum;
                    break;

                case 0 when valueReader.IsFloat32:
                    value.ScalarValue = valueReader.Float32;
                    value.Type = Types.F32;
                    break;

                case 0 when valueReader.IsFloat64:
                    value.ScalarValue = valueReader.Float64;
                    value.Type = Types.F64;
                    break;

                case 0 when valueReader.IsInt16:
                    value.ScalarValue = valueReader.Int16;
                    value.Type = Types.S16;
                    break;

                case 0 when valueReader.IsInt32:
                    value.ScalarValue = valueReader.Int32;
                    value.Type = Types.S32;
                    break;

                case 0 when valueReader.IsInt64:
                    value.ScalarValue = valueReader.Int64;
                    value.Type = Types.S64;
                    break;

                case 0 when valueReader.IsInt8:
                    value.ScalarValue = valueReader.Int8;
                    value.Type = Types.S8;
                    break;

                case 0 when valueReader.IsInterface:
                    value.ScalarValue = null;
                    value.Type = Types.CapabilityPointer;
                    break;

                case 0 when valueReader.IsList:
                    value.RawValue = valueReader.List;
                    value.Type = Types.ListPointer;
                    break;

                case 0 when valueReader.IsStruct:
                    value.RawValue = valueReader.Struct;
                    value.Type = Types.StructPointer;
                    break;

                case 0 when valueReader.IsText:
                    value.ScalarValue = valueReader.Text;
                    value.Type = Types.Text;
                    break;

                case 0 when valueReader.IsUInt16:
                    value.ScalarValue = valueReader.UInt16;
                    value.Type = Types.U16;
                    break;

                case 0 when valueReader.IsUInt32:
                    value.ScalarValue = valueReader.UInt32;
                    value.Type = Types.U32;
                    break;

                case 0 when valueReader.IsUInt64:
                    value.ScalarValue = valueReader.UInt64;
                    value.Type = Types.U64;
                    break;

                case 0 when valueReader.IsUInt8:
                    value.ScalarValue = valueReader.UInt8;
                    value.Type = Types.U8;
                    break;

                case 0 when valueReader.IsVoid:
                    value.Type = Types.Void;
                    break;

                default:
                    throw new NotImplementedException();
            }

            return value;
        }

        void ProcessFields(Schema.Node.Reader reader, TypeDefinition declaringType, List<Field> fields, Pass2State state)
        {
            if (reader.Fields == null)
            {
                return;
            }

            foreach (var fieldReader in reader.Fields)
            {
                var field = new Field()
                {
                    DeclaringType = declaringType,
                    Name = fieldReader.Name,
                    CsName = GetCsName(fieldReader),
                    CodeOrder = fieldReader.CodeOrder
                };

                if (fieldReader.DiscriminantValue != Schema.Field.Reader.NoDiscriminant)
                {
                    field.DiscValue = fieldReader.DiscriminantValue;
                }

                switch (0)
                {
                    case 0 when fieldReader.IsGroup:
                        var def = ProcessTypeDef(fieldReader.Group_TypeId, state, TypeTag.Group);
                        field.Type = Types.FromDefinition(def);
                        def.CsName = field.CsName; // Type definitions for unions are artificially generated.
                                                   // Transfer the C# name of the using field.
                        break;

                    case 0 when fieldReader.IsSlot:
                        field.DefaultValue = ProcessValue(fieldReader.Slot_DefaultValue);
                        field.DefaultValueIsExplicit = fieldReader.Slot_HadExplicitDefault;
                        field.Offset = fieldReader.Slot_Offset;
                        field.Type = ProcessType(fieldReader.Slot_Type, state);
                        field.DefaultValue.Type = field.Type;
                        break;

                    default:
                        throw new NotImplementedException();
                }

                field.InheritFreeGenericParameters();

                fields.Add(field);
            }
        }

        TypeDefinition ProcessInterfaceOrStructTail(TypeDefinition def, Schema.Node.Reader reader, Pass2State state)
        {
            def.IsGeneric = reader.IsGeneric;

            if (def.IsGeneric)
            {
                foreach (var paramReader in reader.Parameters)
                {
                    def.GenericParameters.Add(paramReader.Name);
                }
            }

            ProcessNestedNodes(reader.NestedNodes, state, def.File.IsGenerated);

            ProcessFields(reader, def, def.Fields, state);

            if (reader.IsInterface)
            {
                foreach (var methodReader in reader.Interface_Methods)
                {
                    var method = new Method()
                    {
                        DeclaringInterface = def,
                        Id = def.Methods.Count,
                        Name = methodReader.Name,
                        CsName = GetCsName(methodReader)
                    };
                    foreach (var implicitParameterReader in methodReader.ImplicitParameters)
                    {
                        method.GenericParameters.Add(implicitParameterReader.Name);
                    }
                    state.currentMethod = method;

                    def.Methods.Add(method);

                    var paramNode = IdToNode(methodReader.ParamStructType);
                    var paramType = ProcessParameterList(paramNode, methodReader.ParamBrand, method.Params, state);
                    if (paramType != null)
                    {
                        paramType.SpecialName = SpecialName.MethodParamsStruct;
                        paramType.UsingMethod = method;
                        method.ParamsStruct = Types.FromDefinition(paramType);
                    }
                    else
                    {
                        method.ParamsStruct = method.Params[0].Type;
                    }
                    method.ParamsStruct.InheritFreeParameters(def);
                    method.ParamsStruct.InheritFreeParameters(method);

                    var resultNode = IdToNode(methodReader.ResultStructType);
                    var resultType = ProcessParameterList(resultNode, methodReader.ResultBrand, method.Results, state);
                    if (resultType != null)
                    {
                        resultType.SpecialName = SpecialName.MethodResultStruct;
                        resultType.UsingMethod = method;
                        method.ResultStruct = Types.FromDefinition(resultType);
                    }
                    else
                    {
                        method.ResultStruct = method.Results[0].Type;
                    }
                    method.ResultStruct.InheritFreeParameters(def);
                    method.ResultStruct.InheritFreeParameters(method);
                }

                state.currentMethod = null;
            }
            return def;
        }

        TypeDefinition ProcessStruct(Schema.Node.Reader structReader, TypeDefinition def, Pass2State state)
        {
            def.StructDataWordCount = structReader.Struct_DataWordCount;
            def.StructPointerCount = structReader.Struct_PointerCount;

            if (structReader.Struct_DiscriminantCount > 0)
            {
                def.UnionInfo = new TypeDefinition.DiscriminationInfo(
                    structReader.Struct_DiscriminantCount,
                    16u * structReader.Struct_DiscriminantOffset);
            }

            return ProcessInterfaceOrStructTail(def, structReader, state);
        }

        TypeDefinition ProcessParameterList(Schema.Node.Reader reader, Schema.Brand.Reader brandReader, List<Field> list, Pass2State state)
        {
//# If a named parameter list was specified in the method
//# declaration (rather than a single struct parameter type) then a corresponding struct type is
//# auto-generated. Such an auto-generated type will not be listed in the interface's
//# `nestedNodes` and its `scopeId` will be zero -- it is completely detached from the namespace.
//# (Awkwardly, it does of course inherit generic parameters from the method's scope, which makes
//# this a situation where you can't just climb the scope chain to find where a particular
//# generic parameter was introduced. Making the `scopeId` zero was a mistake.)

            if (!reader.IsStruct)
            {
                throw new InvalidSchemaException("Expected a struct");
            }

            var def = ProcessTypeDef(reader.Id, state, TypeTag.Struct);

            if (reader.ScopeId == 0)
            {
                // Auto-generated => Named parameter list
                foreach (var field in def.Fields) list.Add(field);
                return def;
            }
            else
            {
                // Single, anonymous, struct-typed parameter
                var type = Types.FromDefinition(def);
                ProcessBrand(brandReader, type, state);
                var anon = new Field() { Type = type };
                list.Add(anon);
                return null;
            }
        }

        TypeDefinition ProcessInterface(Schema.Node.Reader ifaceReader, TypeDefinition def, Pass2State state)
        {
            foreach (var superClassReader in ifaceReader.Interface_Superclasses)
            {
                var superClass = Types.FromDefinition(ProcessTypeDef(superClassReader.Id, state, TypeTag.Interface));
                ProcessBrand(superClassReader.Brand, superClass, state);
                def.Superclasses.Add(superClass);
            }

            return ProcessInterfaceOrStructTail(def, ifaceReader, state);
        }

        TypeDefinition ProcessEnum(Schema.Node.Reader enumReader, TypeDefinition def, Pass2State state)
        {
            foreach (var fieldReader in enumReader.Enumerants)
            {
                var field = new Enumerant()
                {
                    TypeDefinition = def,
                    Literal = fieldReader.Name,
                    CsLiteral = GetCsName(fieldReader),
                    CodeOrder = fieldReader.CodeOrder
                };

                if (fieldReader.Ordinal_IsExplicit)
                {
                    field.Ordinal = fieldReader.Ordinal_Explicit;
                }

                def.Enumerants.Add(field);
            }
            return def;
        }

        Constant ProcessConst(Schema.Node.Reader constReader, Constant @const, Pass2State state)
        {
            var value = ProcessValue(constReader.Const_Value);
            value.Type = ProcessType(constReader.Const_Type, state);
            @const.Value = value;
            return @const;
        }

        TypeDefinition ProcessTypeDef(ulong id, Pass2State state, TypeTag tag = default)
        {
            var def = ProcessNode(id, state, true, tag);
            var typeDef = def as TypeDefinition;
            if (typeDef == null)
                throw new ArgumentException(
                    $"Expected node {id.StrId()} to be a TypeDefinition but got {def.GetType().Name} instead.",
                    nameof(id));
            return typeDef;
        }

        IDefinition ProcessNode(ulong id, Pass2State state, bool mustExist, TypeTag tag = default)
        {
            if (!(IdToNode(id, mustExist) is Schema.Node.Reader node)) return null;
            var kind = node.GetKind();
            if (tag == TypeTag.Unknown) tag = kind.GetTypeTag();
            var def = _typeDefMgr.GetExistingDef(id, tag);
            if (state.processedNodes.Contains(id)) return def;
            state.processedNodes.Add(id);

            switch (def)
            {
                case Annotation annotation:
                    return annotation;
                case Constant constant:
                    def.DeclaringElement.Constants.Add(ProcessConst(node, constant, state));
                    return def;
                case TypeDefinition typeDef when kind == NodeKind.Enum:
                    return ProcessEnum(node, typeDef, state);
                case TypeDefinition typeDef when kind == NodeKind.Interface:
                    return ProcessInterface(node, typeDef, state);
                case TypeDefinition typeDef when kind == NodeKind.Struct || kind == NodeKind.Group:
                    return ProcessStruct(node, typeDef, state);
                default:
                    throw new InvalidProgramException($"An unexpected node {node.StrId()} was found during the 2nd schema model building pass.");
            }
        }

        public static SchemaModel Create(Schema.CodeGeneratorRequest.Reader request)
        {
            var model = new SchemaModel(request);
            model.Build();
            return model;
        }
    }

    public enum NodeKind
    {
        Unknown,
        Annotation,
        Const,
        Enum,
        File,
        Interface,
        Struct,
        Group
    }

    public static class SchemaExtensions
    {
        public static string StrId(this Schema.Node.Reader node)
            => $"0x{node.Id:X}";

        public static string StrId(this ulong nodeId)
            => $"0x{nodeId:X}";

        public static NodeKind GetKind(this Schema.Node.Reader node)
        {
            if (node.IsStruct)
                return node.Struct_IsGroup ? NodeKind.Group : NodeKind.Struct;
            if (node.IsInterface) return NodeKind.Interface;
            if (node.IsEnum) return NodeKind.Enum;
            if (node.IsConst) return NodeKind.Const;
            if (node.IsAnnotation) return NodeKind.Annotation;
            if (node.IsFile) return NodeKind.File;
            return NodeKind.Unknown;
        }

        internal static TypeTag GetTypeTag(this NodeKind kind)
        {
            switch (kind)
            {
                case NodeKind.Enum: return TypeTag.Enum;
                case NodeKind.Interface: return TypeTag.Interface;
                case NodeKind.Struct: return TypeTag.Struct;
                case NodeKind.Group: return TypeTag.Group;
                default: return TypeTag.Unknown;
            }
        }

        internal static TypeTag GetTypeTag(this Schema.Node.Reader node)
            => node.GetKind().GetTypeTag();
    }
}
