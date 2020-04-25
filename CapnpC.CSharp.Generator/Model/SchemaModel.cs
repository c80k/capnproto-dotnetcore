using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static CapnpC.CSharp.Generator.Model.SupportedAnnotations;

namespace CapnpC.CSharp.Generator.Model
{
    class SchemaModel
    {
        public const ushort NoDiscriminant = 65535;

        readonly Schema.CodeGeneratorRequest.READER _request;
        readonly List<GenFile> _generatedFiles = new List<GenFile>();
        readonly DefinitionManager _typeDefMgr = new DefinitionManager();

        readonly Dictionary<ulong, Schema.Node.READER> _id2node = new Dictionary<ulong, Schema.Node.READER>();
        readonly Dictionary<ulong, SourceInfo> _id2sourceInfo = new Dictionary<ulong, SourceInfo>();

        public SchemaModel(Schema.CodeGeneratorRequest.READER request)
        {
            _request = request;
        }

        public IReadOnlyList<GenFile> FilesToGenerate => _generatedFiles;

        Schema.Node.READER? IdToNode(ulong id, bool mustExist)
        {
            if (_id2node.TryGetValue(id, out var node))
                return node;
            if (mustExist)
                throw new InvalidSchemaException($"Node with ID {id.StrId()} is required by the codegen backend but is missing.");
            return null;
        }

        Schema.Node.READER IdToNode(ulong id)
        {
            return (Schema.Node.READER)IdToNode(id, true);
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

            foreach (var reader in _request.SourceInfo)
            {
                var sourceInfo = new SourceInfo()
                {
                    DocComment = reader.DocComment,
                    MemberDocComments = reader.Members.Select(m => m.DocComment).ToList()
                };

                _id2sourceInfo.Add(reader.Id, sourceInfo);
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

        void BuildPass1(Dictionary<ulong, Schema.CodeGeneratorRequest.RequestedFile.READER> requestedFiles)
        {
            Pass1State state = new Pass1State()
            {
                unprocessedNodes = new HashSet<ulong>(_id2node.Keys)
            };
            foreach (var node in _id2node.Values.Where(n => n.which == Schema.Node.WHICH.File))
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
            file.EmitDomainClassesAndInterfaces = GetEmitDomainClassesAndInterfaces(node) ?? true;
            file.TypeVisibility = GetTypeVisibility(node) ?? TypeVisibility.Public;
            if (_id2sourceInfo.TryGetValue(node.Id, out var sourceInfo))
            {
                file.HeaderText = GetHeaderText(sourceInfo);
            }
            return ProcessNodePass1(id, name, state) as GenFile;
        }

        IDefinition ProcessNodePass1(ulong id, string name, Pass1State state)
        {
            bool mustExist = state.parent == null || (state.parent as IDefinition).IsGenerated;
            if (!(IdToNode(id, mustExist) is Schema.Node.READER node))
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
            if (processFields && node.Struct.Fields != null)
                foreach (var field in node.Struct.Fields.Where(f => f.which == Schema.Field.WHICH.Group))
                {
                    var group = IdToNode(field.Group.TypeId);
                    if (group.which != Schema.Node.WHICH.Struct || !group.Struct.IsGroup)
                    {
                        throw new InvalidSchemaException($"Expected node with id {group.StrId()} to be a struct definition");
                    }
                    ProcessNodePass1(field.Group.TypeId, field.Name, state);
                }
            if (processInterfaceMethods && node.Interface.Methods != null)
                foreach (var method in node.Interface.Methods)
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

        void BuildPass2(Dictionary<ulong, Schema.CodeGeneratorRequest.RequestedFile.READER> requestedFiles)
        {
            var state = new Pass2State() { processedNodes = new HashSet<ulong>() };
            foreach (var file in _typeDefMgr.Files)
            {
                var node = IdToNode(file.Id);
                ProcessNestedNodes(node.NestedNodes, state, file.IsGenerated);
            }
        }

        void ProcessNestedNodes(IEnumerable<Schema.Node.NestedNode.READER> nestedNodes, Pass2State state, bool mustExist)
        {
            foreach (var nestedNode in nestedNodes)
            {
                ProcessNode(nestedNode.Id, state, mustExist);
            }
        }

        void ProcessBrand(Schema.Brand.READER brandReader, Type type, Pass2State state)
        {
            foreach (var scopeReader in brandReader.Scopes)
            {
                var whatToBind = ProcessTypeDef(scopeReader.ScopeId, state);
                int index = 0;

                switch (scopeReader.which)
                {
                    case Schema.Brand.Scope.WHICH.Bind:
                        foreach (var bindingReader in scopeReader.Bind)
                        {
                            var typeParameter = new GenericParameter()
                            {
                                DeclaringEntity = whatToBind,
                                Index = index++
                            };

                            switch (bindingReader.which)
                            {
                                case Schema.Brand.Binding.WHICH.Type:
                                    type.BindGenericParameter(typeParameter, ProcessType(bindingReader.Type, state));
                                    break;

                                case Schema.Brand.Binding.WHICH.Unbound:
                                    type.BindGenericParameter(typeParameter, Types.FromParameter(typeParameter));
                                    break;
                            }
                        }
                        break;

                    case Schema.Brand.Scope.WHICH.Inherit:
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

        Type ProcessType(Schema.Type.READER typeReader, Pass2State state)
        {
            Type result;

            switch (typeReader.which)
            {
                case Schema.Type.WHICH.AnyPointer:
                    switch (typeReader.AnyPointer.which)
                    {
                        case Schema.Type.anyPointer.WHICH.Parameter:
                            return Types.FromParameter(
                                new GenericParameter()
                                {
                                    DeclaringEntity = ProcessTypeDef(typeReader.AnyPointer.Parameter.ScopeId, state),
                                    Index = typeReader.AnyPointer.Parameter.ParameterIndex
                                });

                        case Schema.Type.anyPointer.WHICH.ImplicitMethodParameter:
                            return Types.FromParameter(
                                new GenericParameter()
                                {
                                    DeclaringEntity = state.currentMethod ?? throw new InvalidOperationException("current method not set"),
                                    Index = typeReader.AnyPointer.ImplicitMethodParameter.ParameterIndex
                                });

                        case Schema.Type.anyPointer.WHICH.Unconstrained:
                            switch (typeReader.AnyPointer.Unconstrained.which)
                            {
                                case Schema.Type.anyPointer.unconstrained.WHICH.AnyKind:
                                    return Types.AnyPointer;

                                case Schema.Type.anyPointer.unconstrained.WHICH.Capability:
                                    return Types.CapabilityPointer;

                                case Schema.Type.anyPointer.unconstrained.WHICH.List:
                                    return Types.ListPointer;

                                case Schema.Type.anyPointer.unconstrained.WHICH.Struct:
                                    return Types.StructPointer;

                                default:
                                    throw new NotImplementedException();
                            }

                        default:
                            throw new NotImplementedException();
                    }

                case Schema.Type.WHICH.Bool:
                    return Types.Bool;

                case Schema.Type.WHICH.Data:
                    return Types.Data;

                case Schema.Type.WHICH.Float64:
                    return Types.F64;

                case Schema.Type.WHICH.Enum:
                    return Types.FromDefinition(ProcessTypeDef(typeReader.Enum.TypeId, state, TypeTag.Enum));

                case Schema.Type.WHICH.Float32:
                    return Types.F32;

                case Schema.Type.WHICH.Int16:
                    return Types.S16;

                case Schema.Type.WHICH.Int32:
                    return Types.S32;

                case Schema.Type.WHICH.Int64:
                    return Types.S64;

                case Schema.Type.WHICH.Int8:
                    return Types.S8;

                case Schema.Type.WHICH.Interface:
                    result = Types.FromDefinition(ProcessTypeDef(typeReader.Interface.TypeId, state, TypeTag.Interface));
                    ProcessBrand(typeReader.Interface.Brand, result, state);
                    return result;

                case Schema.Type.WHICH.List:
                    return Types.List(ProcessType(typeReader.List.ElementType, state));

                case Schema.Type.WHICH.Struct:
                    result = Types.FromDefinition(ProcessTypeDef(typeReader.Struct.TypeId, state, TypeTag.Struct));
                    ProcessBrand(typeReader.Struct.Brand, result, state);
                    return result;

                case Schema.Type.WHICH.Text:
                    return Types.Text;

                case Schema.Type.WHICH.Uint16:
                    return Types.U16;

                case Schema.Type.WHICH.Uint32:
                    return Types.U32;

                case Schema.Type.WHICH.Uint64:
                    return Types.U64;

                case Schema.Type.WHICH.Uint8:
                    return Types.U8;

                case Schema.Type.WHICH.Void:
                    return Types.Void;

                default:
                    throw new NotImplementedException();
            }
        }

        Value ProcessValue(Schema.Value.READER valueReader)
        {
            var value = new Value();

            switch (valueReader.which)
            {
                case Schema.Value.WHICH.AnyPointer:
                    value.ScalarValue = valueReader.AnyPointer;
                    value.Type = Types.AnyPointer;
                    break;

                case Schema.Value.WHICH.Bool:
                    value.ScalarValue = valueReader.Bool;
                    value.Type = Types.Bool;
                    break;

                case Schema.Value.WHICH.Data:
                    value.Items.AddRange(valueReader.Data.Select(Value.Scalar));
                    value.Type = Types.Data;
                    break;

                case Schema.Value.WHICH.Enum:
                    value.ScalarValue = valueReader.Enum;
                    value.Type = Types.AnyEnum;
                    break;

                case Schema.Value.WHICH.Float32:
                    value.ScalarValue = valueReader.Float32;
                    value.Type = Types.F32;
                    break;

                case Schema.Value.WHICH.Float64:
                    value.ScalarValue = valueReader.Float64;
                    value.Type = Types.F64;
                    break;

                case Schema.Value.WHICH.Int16:
                    value.ScalarValue = valueReader.Int16;
                    value.Type = Types.S16;
                    break;

                case Schema.Value.WHICH.Int32:
                    value.ScalarValue = valueReader.Int32;
                    value.Type = Types.S32;
                    break;

                case Schema.Value.WHICH.Int64:
                    value.ScalarValue = valueReader.Int64;
                    value.Type = Types.S64;
                    break;

                case Schema.Value.WHICH.Int8:
                    value.ScalarValue = valueReader.Int8;
                    value.Type = Types.S8;
                    break;

                case Schema.Value.WHICH.Interface:
                    value.ScalarValue = null;
                    value.Type = Types.CapabilityPointer;
                    break;

                case Schema.Value.WHICH.List:
                    value.RawValue = valueReader.List;
                    value.Type = Types.ListPointer;
                    break;

                case Schema.Value.WHICH.Struct:
                    value.RawValue = valueReader.Struct;
                    value.Type = Types.StructPointer;
                    break;

                case Schema.Value.WHICH.Text:
                    value.ScalarValue = valueReader.Text;
                    value.Type = Types.Text;
                    break;

                case Schema.Value.WHICH.Uint16:
                    value.ScalarValue = valueReader.Uint16;
                    value.Type = Types.U16;
                    break;

                case Schema.Value.WHICH.Uint32:
                    value.ScalarValue = valueReader.Uint32;
                    value.Type = Types.U32;
                    break;

                case Schema.Value.WHICH.Uint64:
                    value.ScalarValue = valueReader.Uint64;
                    value.Type = Types.U64;
                    break;

                case Schema.Value.WHICH.Uint8:
                    value.ScalarValue = valueReader.Uint8;
                    value.Type = Types.U8;
                    break;

                case Schema.Value.WHICH.Void:
                    value.Type = Types.Void;
                    break;

                default:
                    throw new NotImplementedException();
            }

            return value;
        }

        void ProcessFields(Schema.Node.READER reader, TypeDefinition declaringType, List<Field> fields, Pass2State state)
        {
            if (reader.Struct.Fields == null)
            {
                return;
            }

            foreach (var fieldReader in reader.Struct.Fields)
            {
                var field = new Field()
                {
                    DeclaringType = declaringType,
                    Name = fieldReader.Name,
                    CsName = GetCsName(fieldReader),
                    CodeOrder = fieldReader.CodeOrder
                };

                if (fieldReader.DiscriminantValue != NoDiscriminant)
                {
                    field.DiscValue = fieldReader.DiscriminantValue;
                }

                switch (fieldReader.which)
                {
                    case Schema.Field.WHICH.Group:
                        var def = ProcessTypeDef(fieldReader.Group.TypeId, state, TypeTag.Group);
                        field.Type = Types.FromDefinition(def);
                        def.CsName = field.CsName; // Type definitions for unions are artificially generated.
                                                   // Transfer the C# name of the using field.
                        break;

                    case Schema.Field.WHICH.Slot:
                        field.DefaultValue = ProcessValue(fieldReader.Slot.DefaultValue);
                        field.DefaultValueIsExplicit = fieldReader.Slot.HadExplicitDefault;
                        field.Offset = fieldReader.Slot.Offset;
                        field.Type = ProcessType(fieldReader.Slot.Type, state);
                        field.DefaultValue.Type = field.Type;
                        break;

                    default:
                        throw new NotImplementedException();
                }

                field.InheritFreeGenericParameters();

                fields.Add(field);
            }
        }

        TypeDefinition ProcessInterfaceOrStructTail(TypeDefinition def, Schema.Node.READER reader, Pass2State state)
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

            if (reader.which == Schema.Node.WHICH.Interface)
            {
                foreach (var methodReader in reader.Interface.Methods)
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

        TypeDefinition ProcessStruct(Schema.Node.READER structReader, TypeDefinition def, Pass2State state)
        {
            def.StructDataWordCount = structReader.Struct.DataWordCount;
            def.StructPointerCount = structReader.Struct.PointerCount;

            if (structReader.Struct.DiscriminantCount > 0)
            {
                def.UnionInfo = new TypeDefinition.DiscriminationInfo(
                    structReader.Struct.DiscriminantCount,
                    16u * structReader.Struct.DiscriminantOffset);
            }

            return ProcessInterfaceOrStructTail(def, structReader, state);
        }

        TypeDefinition ProcessParameterList(Schema.Node.READER reader, Schema.Brand.READER brandReader, List<Field> list, Pass2State state)
        {
//# If a named parameter list was specified in the method
//# declaration (rather than a single struct parameter type) then a corresponding struct type is
//# auto-generated. Such an auto-generated type will not be listed in the interface's
//# `nestedNodes` and its `scopeId` will be zero -- it is completely detached from the namespace.
//# (Awkwardly, it does of course inherit generic parameters from the method's scope, which makes
//# this a situation where you can't just climb the scope chain to find where a particular
//# generic parameter was introduced. Making the `scopeId` zero was a mistake.)

            if (reader.which != Schema.Node.WHICH.Struct)
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

        TypeDefinition ProcessInterface(Schema.Node.READER ifaceReader, TypeDefinition def, Pass2State state)
        {
            foreach (var superClassReader in ifaceReader.Interface.Superclasses)
            {
                var superClass = Types.FromDefinition(ProcessTypeDef(superClassReader.Id, state, TypeTag.Interface));
                ProcessBrand(superClassReader.Brand, superClass, state);
                def.Superclasses.Add(superClass);
            }

            return ProcessInterfaceOrStructTail(def, ifaceReader, state);
        }

        TypeDefinition ProcessEnum(Schema.Node.READER enumReader, TypeDefinition def, Pass2State state)
        {
            foreach (var fieldReader in enumReader.Enum.Enumerants)
            {
                var field = new Enumerant()
                {
                    TypeDefinition = def,
                    Literal = fieldReader.Name,
                    CsLiteral = GetCsName(fieldReader),
                    CodeOrder = fieldReader.CodeOrder
                };

                def.Enumerants.Add(field);
            }
            return def;
        }

        Constant ProcessConst(Schema.Node.READER constReader, Constant @const, Pass2State state)
        {
            var value = ProcessValue(constReader.Const.Value);
            value.Type = ProcessType(constReader.Const.Type, state);
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
            if (!(IdToNode(id, mustExist) is Schema.Node.READER node)) return null;
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

        public static SchemaModel Create(Schema.CodeGeneratorRequest.READER request)
        {
            var model = new SchemaModel(request);
            model.Build();
            return model;
        }
    }

    enum NodeKind
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

    static class SchemaExtensions
    {
        public static string StrId(this Schema.Node.READER node)
            => $"0x{node.Id:X}";

        public static string StrId(this ulong nodeId)
            => $"0x{nodeId:X}";

        public static NodeKind GetKind(this Schema.Node.READER node)
        {
            switch (node.which)
            {
                case Schema.Node.WHICH.Struct: return node.Struct.IsGroup ? NodeKind.Group : NodeKind.Struct;
                case Schema.Node.WHICH.Interface: return NodeKind.Interface;
                case Schema.Node.WHICH.Enum: return NodeKind.Enum;
                case Schema.Node.WHICH.Const: return NodeKind.Const;
                case Schema.Node.WHICH.Annotation: return NodeKind.Annotation;
                case Schema.Node.WHICH.File: return NodeKind.File;
                default: return NodeKind.Unknown;
            }
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

        internal static TypeTag GetTypeTag(this Schema.Node.READER node) => node.GetKind().GetTypeTag();
    }
}
