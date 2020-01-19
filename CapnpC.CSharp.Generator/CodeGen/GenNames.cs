using CapnpC.CSharp.Generator.Model;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CapnpC.CSharp.Generator.CodeGen
{
    enum NameUsage
    {
        Default,
        Interface,
        Proxy,
        Skeleton,
        Namespace
    }

    enum TypeUsage
    {
        NotRelevant,
        DomainClass,
        DomainClassNullable,
        Reader,
        Writer
    }

    class GenNames
    {
        readonly Dictionary<Field, Name> _fieldNameMap = new Dictionary<Field, Name>();

        public NameSyntax TopNamespace { get; set; }
        public Name ReaderStruct { get; }
        public Name ReaderParameter { get; }
        public Name WriterParameter { get; }
        public Name WriterStruct { get; }
        public Name ReaderCreateMethod { get; }
        public Name ReaderContextField { get; }
        public Name ContextParameter { get; }
        public Name GroupReaderContextArg { get; }
        public Name GroupWriterContextArg { get; }
        public Name UnionDiscriminatorEnum { get; }
        public Name UnionDiscriminatorProp { get; }
        public Name UnionDiscriminatorUndefined { get; }
        public Name UnionDiscriminatorField { get; }
        public Name UnionContentField { get; }
        public Name AnonymousParameter { get; }
        public Name CancellationTokenParameter { get; }
        public Name ParamsLocal { get; }
        public Name DeserializerLocal { get; }
        public Name SerializerLocal { get; }
        public Name ResultLocal { get; }
        public Name SerializeMethod { get; }
        public Name ApplyDefaultsMethod { get; }
        public Name InstLocalName { get; }
        public string ParamsStructFormat { get; }
        public string ResultStructFormat { get; }
        public string PropertyNamedLikeTypeRenameFormat { get; }
        public string GenericTypeParameterFormat { get; }
        public string MemberAccessPathNameFormat { get; }
        public Name TaskParameter { get; }
        public Name EagerMethod { get; }
        public Name TypeIdField { get; }
        public string PipeliningExtensionsClassFormat { get; }
        public string ProxyClassFormat { get; }
        public string SkeletonClassFormat { get; }
        public bool NullableEnable { get; set; }
        public GenNames(GeneratorOptions options)
        {
            TopNamespace = new Name(options.TopNamespaceName).IdentifierName;
            ReaderStruct = new Name(options.ReaderStructName);
            WriterStruct = new Name(options.WriterStructName);
            ReaderParameter = new Name(options.ReaderParameterName);
            WriterParameter = new Name(options.WriterParameterName);
            ReaderCreateMethod = new Name(options.ReaderCreateMethodName);
            ReaderContextField = new Name(options.ReaderContextFieldName);
            ContextParameter = new Name(options.ContextParameterName);
            GroupReaderContextArg = new Name(options.GroupReaderContextArgName);
            GroupWriterContextArg = new Name(options.GroupWriterContextArgName);
            UnionDiscriminatorEnum = new Name(options.UnionDiscriminatorEnumName);
            UnionDiscriminatorProp = new Name(options.UnionDiscriminatorPropName);
            UnionDiscriminatorUndefined = new Name(options.UnionDiscriminatorUndefinedName);
            UnionDiscriminatorField = new Name(options.UnionDiscriminatorFieldName);
            UnionContentField = new Name(options.UnionContentFieldName);
            SerializeMethod = new Name(options.SerializeMethodName);
            ApplyDefaultsMethod = new Name(options.ApplyDefaultsMethodName);
            AnonymousParameter = new Name(options.AnonymousParameterName);
            CancellationTokenParameter = new Name(options.CancellationTokenParameterName);
            ParamsLocal = new Name(options.ParamsLocalName);
            DeserializerLocal = new Name(options.DeserializerLocalName);
            SerializerLocal = new Name(options.SerializerLocalName);
            ResultLocal = new Name(options.ResultLocalName);
            InstLocalName = new Name(options.InstLocalName);
            ParamsStructFormat = options.ParamsStructFormat;
            ResultStructFormat = options.ResultStructFormat;
            PropertyNamedLikeTypeRenameFormat = options.PropertyNamedLikeTypeRenameFormat;
            GenericTypeParameterFormat = options.GenericTypeParameterFormat;
            MemberAccessPathNameFormat = options.MemberAccessPathNameFormat;
            TaskParameter = new Name(options.TaskParameterName);
            EagerMethod = new Name(options.EagerMethodName);
            TypeIdField = new Name(options.TypeIdFieldName);
            PipeliningExtensionsClassFormat = options.PipeliningExtensionsClassFormat;
            ProxyClassFormat = options.ProxyClassFormat;
            SkeletonClassFormat = options.SkeletonClassFormat;
        }

        public Name MakeTypeName(TypeDefinition def, NameUsage usage = NameUsage.Default)
        {
            if (def.Tag == TypeTag.Group)
            {
                return new Name(SyntaxHelpers.MakeAllLower(def.Name));
            }
            else
            {
                string name;

                switch (usage)
                {
                    case NameUsage.Default:
                        if (def.Tag == TypeTag.Interface)
                            goto case NameUsage.Interface;

                        switch (def.SpecialName)
                        {
                            case SpecialName.NothingSpecial:
                                name = def.Name;
                                break;

                            case SpecialName.MethodParamsStruct:
                                name = MakeParamsStructName(def.UsingMethod);
                                break;

                            case SpecialName.MethodResultStruct:
                                name = MakeResultStructName(def.UsingMethod);
                                break;

                            default:
                                throw new NotImplementedException();
                        }
                        break;

                    case NameUsage.Namespace:
                        name = def.Name;
                        break;

                    case NameUsage.Interface:
                        name = "I" + def.Name;
                        break;

                    case NameUsage.Proxy:
                        name = string.Format(ProxyClassFormat, def.Name);
                        break;

                    case NameUsage.Skeleton:
                        name = string.Format(SkeletonClassFormat, def.Name);
                        break;

                    default:
                        throw new NotImplementedException();
                }

                return new Name(name);
            }
        }

        public SimpleNameSyntax MakeGenericTypeName(TypeDefinition def, NameUsage usage = NameUsage.Default)
        {
            var name = MakeTypeName(def, usage);

            if (def.GenericParameters.Count > 0)
            {
                return GenericName(name.Identifier)
                    .AddTypeArgumentListArguments(def
                        .GenericParameters
                        .Select(p => GetGenericTypeParameter(p).IdentifierName).ToArray());
            }
            else
            {
                return name.IdentifierName;
            }
        }

        TypeSyntax ResolveGenericParameter(GenericParameter p, Model.Type boundType, TypeDefinition def)
        {
            var type = boundType.ResolveGenericParameter(p);
            return MakeTypeSyntax(type, def, TypeUsage.DomainClass);
        }

        public SimpleNameSyntax MakeGenericTypeName(TypeDefinition def, Model.Type boundType, NameUsage usage = NameUsage.Default)
        {
            var name = MakeTypeName(def, usage);

            if (def.GenericParameters.Count > 0)
            {
                return GenericName(name.Identifier)
                    .AddTypeArgumentListArguments(def
                        .GetLocalTypeParameters()
                        .Select(p => ResolveGenericParameter(p, boundType, def)).ToArray());
            }
            else
            {
                return name.IdentifierName;
            }
        }

        public SimpleNameSyntax MakeGenericTypeNameForAttribute(TypeDefinition def, NameUsage usage)
        {
            var name = MakeTypeName(def, usage);

            if (def.GenericParameters.Count > 0)
            {
                var args = Enumerable.Repeat(OmittedTypeArgument(), def.GenericParameters.Count);
                return GenericName(name.Identifier).AddTypeArgumentListArguments(args.ToArray());
            }
            else
            {
                return name.IdentifierName;
            }
        }

        public static NameSyntax NamespaceName(string[] @namespace)
        {
            NameSyntax ident = null;
            if (@namespace != null)
            {
                ident = IdentifierName(SyntaxHelpers.MakeCamel(@namespace[0]));
                foreach (string name in @namespace.Skip(1))
                {
                    var temp = IdentifierName(SyntaxHelpers.MakeCamel(name));
                    ident = QualifiedName(ident, temp);
                }
            }
            return ident;
        }

        NameSyntax GetNamespaceFor(TypeDefinition def) => NamespaceName(def?.File?.Namespace);

        internal NameSyntax GetQName(Model.Type type, TypeDefinition scope)
        {
            // FIXME: With the help of the 'scope' parameter we will be able to generate abbreviated
            // qualified names. Unfortunately the commented approach is too naive. It will fail if
            // there are multiple objects with identical name up the hierarchy. We will need a more
            // sophisticated algorithm.

            var scopeSet = new HashSet<TypeDefinition>();
            //while (scope != null)
            //{
            //    scopeSet.Add(scope);
            //    scope = scope.DeclaringElement as TypeDefinition;
            //}

            if (type.Definition != null)
            {
                var stack = new Stack<SimpleNameSyntax>();

                var def = type.Definition;
                stack.Push(MakeGenericTypeName(def, type, NameUsage.Default));

                while (def.DeclaringElement is TypeDefinition pdef && !scopeSet.Contains(pdef))
                {
                    stack.Push(MakeGenericTypeName(pdef, type, NameUsage.Namespace));
                    def = pdef;
                }

                var qtype = 
                    GetNamespaceFor(type.Definition)
                    ?? GetNamespaceFor(scope)
                    ?? TopNamespace;

                foreach (var name in stack)
                {
                    qtype = QualifiedName(qtype, name);
                }

                return qtype;
            }
            else
            {
                return GetGenericTypeParameter(type.Parameter.Name).IdentifierName;
            }
        }

        public TypeSyntax MakeListSerializerSyntax(Model.Type elementType, TypeDefinition scope)
        {
            switch (elementType.Tag)
            {
                case TypeTag.AnyPointer:
                case TypeTag.StructPointer:
                case TypeTag.ListPointer:
                    return Type<Capnp.ListOfPointersSerializer<Capnp.DynamicSerializerState>>(true);

                case TypeTag.CapabilityPointer:
                    return Type<Capnp.ListOfCapsSerializer<Capnp.Rpc.BareProxy>>(true);

                case TypeTag.Data:
                    return Type<Capnp.ListOfPointersSerializer<
                        Capnp.ListOfPrimitivesSerializer<byte>>>(true);

                case TypeTag.Enum:
                    return GenericName("ListOfPrimitivesSerializer")
                        .AddTypeArgumentListArguments(MakeTypeSyntax(elementType, scope, TypeUsage.Writer));

                case TypeTag.Group:
                case TypeTag.Struct:
                    return GenericName("ListOfStructsSerializer")
                        .AddTypeArgumentListArguments(MakeTypeSyntax(elementType, scope, TypeUsage.Writer));

                case TypeTag.Interface:
                    return GenericName("ListOfCapsSerializer")
                        .AddTypeArgumentListArguments(MakeTypeSyntax(elementType, scope, TypeUsage.Writer));

                case TypeTag.List:
                    return GenericName("ListOfPointersSerializer")
                        .AddTypeArgumentListArguments(MakeTypeSyntax(elementType, scope, TypeUsage.Writer));

                case TypeTag.Text:
                    return Type<Capnp.ListOfTextSerializer>(true);

                case TypeTag.Void:
                    return Type<Capnp.ListOfEmptySerializer>(true);

                case TypeTag.Bool:
                    return Type<Capnp.ListOfBitsSerializer>(true);

                case TypeTag.F32:
                    return Type<Capnp.ListOfPrimitivesSerializer<float>>(true);

                case TypeTag.F64:
                    return Type<Capnp.ListOfPrimitivesSerializer<double>>(true);

                case TypeTag.S8:
                    return Type<Capnp.ListOfPrimitivesSerializer<sbyte>>(true);

                case TypeTag.U8:
                    return Type<Capnp.ListOfPrimitivesSerializer<byte>>(true);

                case TypeTag.S16:
                    return Type<Capnp.ListOfPrimitivesSerializer<short>>(true);

                case TypeTag.U16:
                case TypeTag.AnyEnum:
                    return Type<Capnp.ListOfPrimitivesSerializer<ushort>>(true);

                case TypeTag.S32:
                    return Type<Capnp.ListOfPrimitivesSerializer<int>>(true);

                case TypeTag.U32:
                    return Type<Capnp.ListOfPrimitivesSerializer<uint>>(true);

                case TypeTag.S64:
                    return Type<Capnp.ListOfPrimitivesSerializer<long>>(true);

                case TypeTag.U64:
                    return Type<Capnp.ListOfPrimitivesSerializer<ulong>>(true);

                default:
                    throw new NotImplementedException("Unexpected type tag, don't know how to deal with this");
            }
        }

        TypeSyntax MaybeNullableValueType(TypeSyntax typeSyntax, TypeUsage usage)
        {
            switch (usage)
            {
                case TypeUsage.DomainClassNullable:
                    return NullableType(typeSyntax);

                default:
                    return typeSyntax;
            }
        }

        public TypeSyntax MakeTypeSyntax(Model.Type type, TypeDefinition scope, TypeUsage usage)
        {
            bool nonNullable = usage != TypeUsage.DomainClassNullable;

            switch (type.Tag)
            {
                case TypeTag.AnyEnum:
                    return MaybeNullableValueType(Type<ushort>(), usage);

                case TypeTag.CapabilityPointer:
                    if (type.Parameter != null)
                    {
                        return nonNullable ? GetQName(type, scope) :
                            MakeNullableType(GetQName(type, scope));
                    }
                    else
                    {
                        return Type<Capnp.Rpc.BareProxy>(nonNullable);
                    }

                case TypeTag.AnyPointer:
                case TypeTag.StructPointer:
                    switch (usage)
                    {
                        case TypeUsage.Reader:
                            return Type<Capnp.DeserializerState>();

                        case TypeUsage.Writer:
                            return Type<Capnp.DynamicSerializerState>();

                        case TypeUsage.DomainClass:
                        case TypeUsage.DomainClassNullable:
                            if (type.Parameter != null)
                            {
                                return nonNullable ?
                                    GetQName(type, scope) :
                                    MakeNullableType(GetQName(type, scope));
                            }
                            else
                            {
                                return Type<object>(nonNullable);
                            }

                        default:
                            throw new NotImplementedException();
                    }

                case TypeTag.Bool:
                    return MaybeNullableValueType(Type<bool>(), usage);

                case TypeTag.Data:
                    switch (usage)
                    {
                        case TypeUsage.Reader:
                        case TypeUsage.DomainClass:
                        case TypeUsage.DomainClassNullable:
                            return Type<IReadOnlyList<byte>>(nonNullable);

                        case TypeUsage.Writer:
                            return Type<Capnp.ListOfPrimitivesSerializer<byte>>(true);

                        default:
                            throw new NotImplementedException();
                    }

                case TypeTag.Enum:
                    return MaybeNullableValueType(GetQName(type, scope), usage);

                case TypeTag.Interface:
                    return GetQName(type, scope);

                case TypeTag.Struct:
                case TypeTag.Group:
                    switch (usage)
                    {
                        case TypeUsage.Writer:
                            return QualifiedName(GetQName(type, scope), WriterStruct.IdentifierName);

                        case TypeUsage.Reader:
                            return QualifiedName(GetQName(type, scope), ReaderStruct.IdentifierName);

                        case TypeUsage.DomainClass:
                            return GetQName(type, scope);

                        case TypeUsage.DomainClassNullable:
                            return MakeNullableType(GetQName(type, scope));

                        default:
                            throw new NotImplementedException();
                    }

                case TypeTag.F32:
                    return MaybeNullableValueType(Type<float>(), usage);

                case TypeTag.F64:
                    return MaybeNullableValueType(Type<double>(), usage);

                case TypeTag.List when type.ElementType.Tag == TypeTag.Void && usage != TypeUsage.Writer:
                    return MaybeNullableValueType(Type<int>(), usage);

                case TypeTag.List:
                    switch (usage)
                    {
                        case TypeUsage.Writer:
                            return MakeListSerializerSyntax(type.ElementType, scope);

                        case TypeUsage.Reader:
                            return GenericName(Identifier("IReadOnlyList"))
                                .AddTypeArgumentListArguments(MakeTypeSyntax(type.ElementType, scope, TypeUsage.Reader));

                        case TypeUsage.DomainClass:
                        case TypeUsage.DomainClassNullable:
                            return GenericName(Identifier("IReadOnlyList"))
                                .AddTypeArgumentListArguments(MakeTypeSyntax(type.ElementType, scope, usage));

                        default:
                            throw new NotImplementedException();
                    }

                case TypeTag.ListPointer:
                    switch (usage)
                    {
                        case TypeUsage.Writer:
                            return Type<Capnp.SerializerState>();

                        case TypeUsage.Reader:
                            return Type<IReadOnlyList<Capnp.DeserializerState>>();

                        case TypeUsage.DomainClass:
                            return Type<IReadOnlyList<object>>(false);

                        case TypeUsage.DomainClassNullable:
                            return Type<IReadOnlyList<object>>(true);

                        default:
                            throw new NotImplementedException();
                    }

                case TypeTag.S16:
                    return MaybeNullableValueType(Type<short>(), usage);

                case TypeTag.S32:
                    return MaybeNullableValueType(Type<int>(), usage);

                case TypeTag.S64:
                    return MaybeNullableValueType(Type<long>(), usage);

                case TypeTag.S8:
                    return MaybeNullableValueType(Type<sbyte>(), usage);

                case TypeTag.Text:
                    return Type<string>(nonNullable);

                case TypeTag.U16:
                    return MaybeNullableValueType(Type<ushort>(), usage);

                case TypeTag.U32:
                    return MaybeNullableValueType(Type<uint>(), usage);

                case TypeTag.U64:
                    return MaybeNullableValueType(Type<ulong>(), usage);

                case TypeTag.U8:
                    return MaybeNullableValueType(Type<byte>(), usage);

                case TypeTag.Void:
                    return PredefinedType(Token(SyntaxKind.VoidKeyword));

                default:
                    throw new NotImplementedException("Unexpected type tag, don't know how to deal with this");
            }
        }

        public string MakeParamsStructName(Method method)
        {
            return string.Format(ParamsStructFormat, method.Name);
        }

        public string MakeResultStructName(Method method)
        {
            return string.Format(ResultStructFormat, method.Name);
        }

        public Name GetCodeIdentifier(Method method)
        {
            return new Name(SyntaxHelpers.MakeCamel(method.Name));
        }

        public Name GetCodeIdentifier(Field field)
        {
            if (_fieldNameMap.TryGetValue(field, out var name))
            {
                return name;
            }

            var def = field.DeclaringType;

            if (def == null)
            {
                // Method parameters are internally represented with the same class "Field".
                // They do not have a declaring type. Anyway, they don't suffer from the field-name-equals-nested-type-name problem.
                return new Name(SyntaxHelpers.MakeCamel(field.Name));
            }

            var typeNames = new HashSet<Name>(def.NestedTypes.Select(t => MakeTypeName(t)));
            typeNames.Add(MakeTypeName(def));

            foreach (var member in def.Fields)
            {
                var memberName = new Name(SyntaxHelpers.MakeCamel(member.Name));

                while (typeNames.Contains(memberName))
                {
                    memberName = new Name(string.Format(PropertyNamedLikeTypeRenameFormat, memberName.ToString()));
                }

                _fieldNameMap.Add(member, memberName);
            }

            return _fieldNameMap[field];
        }

        public Name GetGenericTypeParameter(string name)
        {
            return new Name(string.Format(GenericTypeParameterFormat, name));
        }

        public Name MakePipeliningSupportExtensionMethodName(IReadOnlyList<Field> path)
        {
            if (path.Count == 1 && path[0].Offset == 0)
                return EagerMethod;
            else
                return new Name(string.Join("_", path.Select(f => GetCodeIdentifier(f).ToString())));
        }

        public Name MakePipeliningSupportExtensionClassName(GenFile file)
        {
            return new Name(string.Format(PipeliningExtensionsClassFormat, Path.GetFileNameWithoutExtension(file.Name).Replace(".", "_")));
        }

        public Name MakeMemberAccessPathFieldName(Method method, IReadOnlyList<Field> path)
        {
            var nameList = new Stack<string>();
            var cur = method.DeclaringInterface.DeclaringElement;
            do
            {
                switch (cur)
                {
                    case TypeDefinition def:
                        nameList.Push(def.Name);
                        cur = def.DeclaringElement;
                        break;

                    case GenFile file:
                        if (file.Namespace != null)
                        {
                            foreach (string id in file.Namespace.Reverse())
                            {
                                nameList.Push(id);
                            }
                        }
                        cur = null;
                        break;

                    default:
                        throw new NotImplementedException("Unexpected element in definition hierarchy of method. Not a type, not a file.");
                }
            } while (cur != null);

            return new Name(string.Format(MemberAccessPathNameFormat,
                string.Join("_", nameList),
                method.DeclaringInterface.Name,
                method.Name,
                MakePipeliningSupportExtensionMethodName(path)));
        }

        public TypeSyntax MakeNullableType(TypeSyntax type)
        {
            return NullableEnable ?
                NullableType(type) :
                type;

        }

        public TypeSyntax Type<T>(bool nonNullable = false)
        {
            return NullableEnable && !typeof(T).IsValueType && !nonNullable ?
                NullableType(SyntaxHelpers.NonNullableType<T>()) :
                SyntaxHelpers.NonNullableType<T>();                
        }

        public ClassOrStructConstraintSyntax MakeNullableClassConstraint() => ClassOrStructConstraint(SyntaxKind.ClassConstraint);
    }
}
