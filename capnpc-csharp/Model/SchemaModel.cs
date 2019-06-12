using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CapnpC.Model
{
    class SchemaModel
    {
        readonly Schema.CodeGeneratorRequest.Reader _request;
        readonly List<GenFile> _files = new List<GenFile>();
        readonly Stack<IHasNestedDefinitions> _typeNest = new Stack<IHasNestedDefinitions>();
        readonly TypeDefinitionManager _typeDefMgr = new TypeDefinitionManager();
        Method _currentMethod;

        Dictionary<ulong, Schema.Node.Reader> _id2node;

        public SchemaModel(Schema.CodeGeneratorRequest.Reader request)
        {
            _request = request;
        }

        public IReadOnlyList<GenFile> FilesToGenerate => _files;

        Schema.Node.Reader IdToNode(ulong id)
        {
            try
            {
                return _id2node[id];
            }
            catch (KeyNotFoundException)
            {
                throw new InvalidSchemaException($"Node with ID {id} is missing");
            }
        }

        void Build()
        {
            if (_request.Nodes == null || _request.Nodes.Count == 0)
            {
                throw new InvalidSchemaException("No nodes, nothing to generate");
            }

            try
            {
                _id2node = _request.Nodes.ToDictionary(n => n.Id);
            }
            catch (ArgumentException)
            {
                throw new InvalidSchemaException("Nodes with duplicate IDs detected");
            }

            foreach (var reqFile in _request.RequestedFiles)
            {
                var file = new GenFile()
                {
                    Name = reqFile.Filename
                };

                _files.Add(file);
                _typeNest.Push(file);

                var fileNode = IdToNode(reqFile.Id);

                if (!fileNode.IsFile)
                    throw new InvalidSchemaException("Expected a file node");

                ProcessFile(fileNode);

                _typeNest.Pop();
            }
        }

        void ProcessFile(Schema.Node.Reader fileReader)
        {
            foreach (var annotation in fileReader.Annotations)
            {
                if (annotation.Id == 0xb9c6f99ebf805f2c) // Cxx namespace
                {
                    ((GenFile)_typeNest.Peek()).Namespace = annotation.Value.Text.Split("::");
                }
            }

            foreach (var nestedNode in fileReader.NestedNodes)
            {
                var node = IdToNode(nestedNode.Id);

                ProcessNode(node, nestedNode.Name);
            }
        }

        TypeDefinition GetOrCreateTypeDef(ulong id, TypeTag tag) => _typeDefMgr.GetOrCreate(id, tag);
        TypeDefinition GetGroupTypeDef(ulong id, string name)
        {
            var nodeReader = _id2node[id];

            if (!nodeReader.IsStruct)
                throw new InvalidSchemaException($"Expected node with id {id} to be a struct definition");

            return ProcessStruct(nodeReader, name);
        }
        void ProcessBrand(Schema.Brand.Reader brandReader, Type type)
        {
            foreach (var scopeReader in brandReader.Scopes)
            {
                var whatToBind = GetOrCreateTypeDef(scopeReader.ScopeId, TypeTag.Unknown);
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
                                    type.BindGenericParameter(typeParameter, ProcessType(bindingReader.Type));
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
        Type ProcessType(Schema.Type.Reader typeReader)
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
                                    DeclaringEntity = GetOrCreateTypeDef(typeReader.AnyPointer_Parameter_ScopeId, TypeTag.Unknown),
                                    Index = typeReader.AnyPointer_Parameter_ParameterIndex
                                });

                        case 0 when typeReader.AnyPointer_IsImplicitMethodParameter:
                            return Types.FromParameter(
                                new GenericParameter()
                                {
                                    DeclaringEntity = _currentMethod ?? throw new InvalidOperationException("current method not set"),
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
                    return Types.FromDefinition(GetOrCreateTypeDef(typeReader.Enum_TypeId, TypeTag.Enum));

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
                    result = Types.FromDefinition(GetOrCreateTypeDef(typeReader.Interface_TypeId, TypeTag.Interface));
                    ProcessBrand(typeReader.Interface_Brand, result);
                    return result;

                case 0 when typeReader.IsList:
                    return Types.List(ProcessType(typeReader.List_ElementType));

                case 0 when typeReader.IsStruct:
                    result = Types.FromDefinition(GetOrCreateTypeDef(typeReader.Struct_TypeId, TypeTag.Struct));
                    ProcessBrand(typeReader.Struct_Brand, result);
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

        void ProcessFields(Schema.Node.Reader reader, TypeDefinition declaringType, List<Field> fields)
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
                    CodeOrder = fieldReader.CodeOrder
                };

                if (fieldReader.DiscriminantValue != Schema.Field.Reader.NoDiscriminant)
                {
                    field.DiscValue = fieldReader.DiscriminantValue;
                }

                switch (0)
                {
                    case 0 when fieldReader.IsGroup:
                        field.Type = Types.FromDefinition(GetGroupTypeDef(
                            fieldReader.Group_TypeId, fieldReader.Name));
                        break;

                    case 0 when fieldReader.IsSlot:
                        field.DefaultValue = ProcessValue(fieldReader.Slot_DefaultValue);
                        field.DefaultValueIsExplicit = fieldReader.Slot_HadExplicitDefault;
                        field.Offset = fieldReader.Slot_Offset;
                        field.Type = ProcessType(fieldReader.Slot_Type);
                        field.DefaultValue.Type = field.Type;
                        break;

                    default:
                        throw new NotImplementedException();
                }

                field.InheritFreeGenericParameters();

                fields.Add(field);
            }
        }

        void ProcessInterfaceOrStructTail(TypeDefinition def, Schema.Node.Reader reader)
        {
            def.IsGeneric = reader.IsGeneric;

            if (def.IsGeneric)
            {
                foreach (var paramReader in reader.Parameters)
                {
                    def.GenericParameters.Add(paramReader.Name);
                }
            }

            _typeNest.Push(def);

            if (reader.NestedNodes != null)
            {
                foreach (var nestedReader in reader.NestedNodes)
                {
                    var node = IdToNode(nestedReader.Id);
                    ProcessNode(node, nestedReader.Name);
                }
            }

            ProcessFields(reader, def, def.Fields);

            if (reader.IsInterface)
            {
                foreach (var methodReader in reader.Interface_Methods)
                {
                    var method = new Method()
                    {
                        DeclaringInterface = def,
                        Id = def.Methods.Count,
                        Name = methodReader.Name
                    };
                    foreach (var implicitParameterReader in methodReader.ImplicitParameters)
                    {
                        method.GenericParameters.Add(implicitParameterReader.Name);
                    }
                    _currentMethod = method;

                    def.Methods.Add(method);

                    var paramNode = IdToNode(methodReader.ParamStructType);
                    var paramType = ProcessParameterList(paramNode, methodReader.ParamBrand, method.Params);
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
                    var resultType = ProcessParameterList(resultNode, methodReader.ResultBrand, method.Results);
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

                _currentMethod = null;
            }

            _typeNest.Pop();
        }

        TypeDefinition ProcessStruct(Schema.Node.Reader structReader, string name)
        {
            var def = GetOrCreateTypeDef(
                structReader.Id, 
                structReader.Struct_IsGroup ? TypeTag.Group : TypeTag.Struct);

            def.DeclaringElement = _typeNest.Peek();
            if (structReader.Struct_IsGroup)
                ((TypeDefinition)def.DeclaringElement).NestedGroups.Add(def);
            else
                def.DeclaringElement.NestedTypes.Add(def);
            def.Name = name;
            def.StructDataWordCount = structReader.Struct_DataWordCount;
            def.StructPointerCount = structReader.Struct_PointerCount;

            if (structReader.Struct_DiscriminantCount > 0)
            {
                def.UnionInfo = new TypeDefinition.DiscriminationInfo(
                    structReader.Struct_DiscriminantCount,
                    16u * structReader.Struct_DiscriminantOffset);
            }

            ProcessInterfaceOrStructTail(def, structReader);

            return def;
        }

        TypeDefinition ProcessParameterList(Schema.Node.Reader reader, Schema.Brand.Reader brandReader, List<Field> list)
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

            if (reader.ScopeId == 0)
            {
                // Auto-generated => Named parameter list
                ProcessFields(reader, null, list);
                return ProcessStruct(reader, null);
            }
            else
            {
                // Single, anonymous, struct-typed parameter
                var def = GetOrCreateTypeDef(reader.Id, TypeTag.Struct);
                var type = Types.FromDefinition(def);
                ProcessBrand(brandReader, type);
                var anon = new Field() { Type = type };
                list.Add(anon);
                return null;
            }
        }

        TypeDefinition ProcessInterface(Schema.Node.Reader ifaceReader, string name)
        {
            var def = GetOrCreateTypeDef(
                ifaceReader.Id,
                TypeTag.Interface);

            def.DeclaringElement = _typeNest.Peek();
            def.DeclaringElement.NestedTypes.Add(def);
            def.Name = name;

            foreach (var superClassReader in ifaceReader.Interface_Superclasses)
            {
                var superClass = GetOrCreateTypeDef(
                    superClassReader.Id,
                    TypeTag.Interface);

                def.Superclasses.Add(Types.FromDefinition(superClass));
            }

            ProcessInterfaceOrStructTail(def, ifaceReader);

            return def;
        }

        void ProcessEnum(Schema.Node.Reader enumReader, string name)
        {
            var def = GetOrCreateTypeDef(enumReader.Id, TypeTag.Enum);

            def.DeclaringElement = _typeNest.Peek();
            def.DeclaringElement.NestedTypes.Add(def);
            def.Name = name;

            _typeNest.Push(def);

            foreach (var fieldReader in enumReader.Enumerants)
            {
                var field = new Enumerant()
                {
                    TypeDefinition = def,
                    Literal = fieldReader.Name,
                    CodeOrder = fieldReader.CodeOrder
                };

                if (fieldReader.Ordinal_IsExplicit)
                {
                    field.Ordinal = fieldReader.Ordinal_Explicit;
                }

                def.Enumerants.Add(field);
            }

            _typeNest.Pop();
        }

        void ProcessConst(Schema.Node.Reader constReader, string name)
        {
            var value = ProcessValue(constReader.Const_Value);
            value.Type = ProcessType(constReader.Const_Type);

            _typeNest.Peek().Constants.Add(value);
        }

        void ProcessNode(Schema.Node.Reader node, string name)
        {
            switch (0)
            {
                case 0 when node.IsAnnotation:
                    break;

                case 0 when node.IsConst:
                    ProcessConst(node, name);
                    break;

                case 0 when node.IsEnum:
                    ProcessEnum(node, name);
                    break;

                case 0 when node.IsFile:
                    throw new InvalidSchemaException("Did not expect file nodes to appear as nested nodes");

                case 0 when node.IsInterface:
                    ProcessInterface(node, name);
                    break;

                case 0 when node.IsStruct:
                    ProcessStruct(node, name);
                    break;

                default:
                    throw new InvalidSchemaException($"Don't know how to process node {node.DisplayName}");
            }
        }

        public static SchemaModel Create(Schema.CodeGeneratorRequest.Reader request)
        {
            var model = new SchemaModel(request);
            model.Build();
            return model;
        }
    }
}
