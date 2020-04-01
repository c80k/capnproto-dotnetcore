using CapnpC.CSharp.Generator.Model;
using Microsoft.CodeAnalysis;
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
        Reader,
        Writer
    }

    enum Nullability
    {
        NonNullable,
        NullableRefAndValue,
        NullableRef
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
        public Name AwaitProxy { get; }
        public bool NullableEnable { get; set; }
        public bool EmitDomainClassesAndInterfaces { get; set; }
        public SupportedAnnotations.TypeVisibility TypeVisibility { get; set; }
        public string GeneratorToolName { get; }
        public string GeneratorToolVersion { get; }

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
            AwaitProxy = new Name(options.AwaitProxyName);
            GeneratorToolName = options.GeneratorToolName;
            GeneratorToolVersion = options.GeneratorToolVersion;
        }

        public Name MakeTypeName(TypeDefinition def, NameUsage usage = NameUsage.Default)
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
                            name = GetCodeIdentifier(def);
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
                    name = GetCodeIdentifier(def);
                    break;

                case NameUsage.Interface:
                    name = "I" + GetCodeIdentifier(def);
                    break;

                case NameUsage.Proxy:
                    name = string.Format(ProxyClassFormat, GetCodeIdentifier(def));
                    break;

                case NameUsage.Skeleton:
                    name = string.Format(SkeletonClassFormat, GetCodeIdentifier(def));
                    break;

                default:
                    throw new NotImplementedException();
            }

            return new Name(name);
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
            return MakeTypeSyntax(type, def, TypeUsage.DomainClass, Nullability.NonNullable);
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
                ident = IdentifierName(SyntaxHelpers.MakeUpperCamel(@namespace[0]));
                foreach (string name in @namespace.Skip(1))
                {
                    var temp = IdentifierName(SyntaxHelpers.MakeUpperCamel(name));
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
                    return Type<Capnp.ListOfPointersSerializer<Capnp.DynamicSerializerState>>(Nullability.NonNullable);

                case TypeTag.CapabilityPointer:
                    return Type<Capnp.ListOfCapsSerializer<Capnp.Rpc.BareProxy>>(Nullability.NonNullable);

                case TypeTag.Data:
                    return Type<Capnp.ListOfPointersSerializer<
                        Capnp.ListOfPrimitivesSerializer<byte>>>(Nullability.NonNullable);

                case TypeTag.Enum:
                    return GenericName("ListOfPrimitivesSerializer")
                        .AddTypeArgumentListArguments(MakeTypeSyntax(elementType, scope, TypeUsage.Writer, Nullability.NonNullable));

                case TypeTag.Group:
                case TypeTag.Struct:
                    return GenericName("ListOfStructsSerializer")
                        .AddTypeArgumentListArguments(MakeTypeSyntax(elementType, scope, TypeUsage.Writer, Nullability.NonNullable));

                case TypeTag.Interface:
                    return GenericName("ListOfCapsSerializer")
                        .AddTypeArgumentListArguments(MakeTypeSyntax(elementType, scope, TypeUsage.Writer, Nullability.NonNullable));

                case TypeTag.List:
                    return GenericName("ListOfPointersSerializer")
                        .AddTypeArgumentListArguments(MakeTypeSyntax(elementType, scope, TypeUsage.Writer, Nullability.NonNullable));

                case TypeTag.Text:
                    return Type<Capnp.ListOfTextSerializer>(Nullability.NonNullable);

                case TypeTag.Void:
                    return Type<Capnp.ListOfEmptySerializer>(Nullability.NonNullable);

                case TypeTag.Bool:
                    return Type<Capnp.ListOfBitsSerializer>(Nullability.NonNullable);

                case TypeTag.F32:
                    return Type<Capnp.ListOfPrimitivesSerializer<float>>(Nullability.NonNullable);

                case TypeTag.F64:
                    return Type<Capnp.ListOfPrimitivesSerializer<double>>(Nullability.NonNullable);

                case TypeTag.S8:
                    return Type<Capnp.ListOfPrimitivesSerializer<sbyte>>(Nullability.NonNullable);

                case TypeTag.U8:
                    return Type<Capnp.ListOfPrimitivesSerializer<byte>>(Nullability.NonNullable);

                case TypeTag.S16:
                    return Type<Capnp.ListOfPrimitivesSerializer<short>>(Nullability.NonNullable);

                case TypeTag.U16:
                case TypeTag.AnyEnum:
                    return Type<Capnp.ListOfPrimitivesSerializer<ushort>>(Nullability.NonNullable);

                case TypeTag.S32:
                    return Type<Capnp.ListOfPrimitivesSerializer<int>>(Nullability.NonNullable);

                case TypeTag.U32:
                    return Type<Capnp.ListOfPrimitivesSerializer<uint>>(Nullability.NonNullable);

                case TypeTag.S64:
                    return Type<Capnp.ListOfPrimitivesSerializer<long>>(Nullability.NonNullable);

                case TypeTag.U64:
                    return Type<Capnp.ListOfPrimitivesSerializer<ulong>>(Nullability.NonNullable);

                default:
                    throw new NotImplementedException("Unexpected type tag, don't know how to deal with this");
            }
        }

        TypeSyntax MaybeNullableValueType(TypeSyntax typeSyntax, Nullability nullability)
        {
            switch (nullability)
            {
                case Nullability.NullableRefAndValue:
                    return NullableType(typeSyntax);

                default:
                    return typeSyntax;
            }
        }

        TypeSyntax MaybeNullableRefType(TypeSyntax typeSyntax, Nullability nullability)
        {
            if (!NullableEnable) 
                return typeSyntax;

            switch (nullability)
            {
                case Nullability.NullableRef:
                case Nullability.NullableRefAndValue:
                    return NullableType(typeSyntax);

                default:
                    return typeSyntax;
            }
        }

        public Nullability GetDefaultElementTypeNullability(Model.Type type)
        {
            switch (type.Tag)
            {
                case TypeTag.Data:
                case TypeTag.Text:
                case TypeTag.Interface:
                case TypeTag.List:
                case TypeTag.ListPointer:
                case TypeTag.StructPointer:
                    return Nullability.NullableRef;

                default:
                    return Nullability.NonNullable;
            }
        }

        public TypeSyntax MakeTypeSyntax(Model.Type type, TypeDefinition scope, TypeUsage usage, Nullability nullability)
        {
            switch (type.Tag)
            {
                case TypeTag.AnyEnum:
                    return Type<ushort>(nullability);

                case TypeTag.CapabilityPointer:
                    if (type.Parameter != null)
                        return MaybeNullableRefType(GetQName(type, scope), nullability);
                    else
                        return Type<Capnp.Rpc.BareProxy>(nullability);

                case TypeTag.AnyPointer:
                case TypeTag.StructPointer:
                    switch (usage)
                    {
                        case TypeUsage.Reader:
                            return Type<Capnp.DeserializerState>(Nullability.NonNullable);

                        case TypeUsage.Writer:
                            return Type<Capnp.DynamicSerializerState>(Nullability.NullableRef);

                        case TypeUsage.DomainClass when type.Parameter == null:
                            return Type<object>(nullability);

                        case TypeUsage.DomainClass when nullability == Nullability.NonNullable:
                            return GetQName(type, scope);

                        case TypeUsage.DomainClass:
                            return MakeNullableRefType(GetQName(type, scope));

                        default:
                            throw new NotImplementedException();
                    }

                case TypeTag.Bool:
                    return Type<bool>(nullability);

                case TypeTag.Data:
                    switch (usage)
                    {
                        case TypeUsage.Reader:
                        case TypeUsage.DomainClass:
                            return Type<IReadOnlyList<byte>>(nullability);

                        case TypeUsage.Writer:
                            return Type<Capnp.ListOfPrimitivesSerializer<byte>>(nullability);

                        default:
                            throw new NotImplementedException();
                    }

                case TypeTag.Enum:
                    return MaybeNullableValueType(GetQName(type, scope), nullability);

                case TypeTag.Interface:
                    return MaybeNullableRefType(GetQName(type, scope), nullability);

                case TypeTag.Struct:
                case TypeTag.Group:
                    switch (usage)
                    {
                        case TypeUsage.Writer:
                            return QualifiedName(GetQName(type, scope), WriterStruct.IdentifierName);

                        case TypeUsage.Reader:
                            return QualifiedName(GetQName(type, scope), ReaderStruct.IdentifierName);

                        case TypeUsage.DomainClass when nullability == Nullability.NonNullable:
                            return GetQName(type, scope);

                        case TypeUsage.DomainClass:
                            return MakeNullableRefType(GetQName(type, scope));

                        default:
                            throw new NotImplementedException();
                    }

                case TypeTag.F32:
                    return Type<float>(nullability);

                case TypeTag.F64:
                    return Type<double>(nullability);

                case TypeTag.List when type.ElementType.Tag == TypeTag.Void && usage != TypeUsage.Writer:
                    return Type<int>(nullability);

                case TypeTag.List:
                    switch (usage)
                    {
                        case TypeUsage.Writer:
                            return MaybeNullableRefType(MakeListSerializerSyntax(type.ElementType, scope), nullability);

                        case TypeUsage.Reader:
                            return MaybeNullableRefType(GenericName(Identifier("IReadOnlyList"))
                                .AddTypeArgumentListArguments(MakeTypeSyntax(type.ElementType, scope, TypeUsage.Reader,
                                    GetDefaultElementTypeNullability(type.ElementType))), nullability);

                        case TypeUsage.DomainClass:
                            return MaybeNullableRefType(GenericName(Identifier("IReadOnlyList"))
                                .AddTypeArgumentListArguments(MakeTypeSyntax(type.ElementType, scope, TypeUsage.DomainClass,
                                    GetDefaultElementTypeNullability(type.ElementType))), nullability);

                        default:
                            throw new NotImplementedException();
                    }

                case TypeTag.ListPointer:
                    switch (usage)
                    {
                        case TypeUsage.Writer:
                            return Type<Capnp.SerializerState>(Nullability.NonNullable);

                        case TypeUsage.Reader:
                            return Type<IReadOnlyList<Capnp.DeserializerState>>(Nullability.NonNullable);

                        case TypeUsage.DomainClass when nullability == Nullability.NonNullable:
                            return GenericName(Identifier("IReadOnlyList"))
                                .AddTypeArgumentListArguments(Type<object>(Nullability.NullableRef));

                        case TypeUsage.DomainClass:
                            return MakeNullableRefType(GenericName(Identifier("IReadOnlyList"))
                                .AddTypeArgumentListArguments(Type<object>(Nullability.NullableRef)));

                        default:
                            throw new NotImplementedException();
                    }

                case TypeTag.S16:
                    return Type<short>(nullability);

                case TypeTag.S32:
                    return Type<int>(nullability);

                case TypeTag.S64:
                    return Type<long>(nullability);

                case TypeTag.S8:
                    return Type<sbyte>(nullability);

                case TypeTag.Text:
                    return Type<string>(nullability);

                case TypeTag.U16:
                    return Type<ushort>(nullability);

                case TypeTag.U32:
                    return Type<uint>(nullability);

                case TypeTag.U64:
                    return Type<ulong>(nullability);

                case TypeTag.U8:
                    return Type<byte>(nullability);

                case TypeTag.Void:
                    return PredefinedType(Token(SyntaxKind.VoidKeyword));

                default:
                    throw new NotImplementedException("Unexpected type tag, don't know how to deal with this");
            }
        }

        public string MakeParamsStructName(Method method)
        {
            return string.Format(ParamsStructFormat, GetCodeIdentifier(method));
        }

        public string MakeResultStructName(Method method)
        {
            return string.Format(ResultStructFormat, GetCodeIdentifier(method));
        }

        public Name GetCodeIdentifier(Method method)
        {
            return new Name(method.CsName ?? IdentifierRenamer.ToNonKeyword(SyntaxHelpers.MakeUpperCamel(method.Name)));
        }

        string GetCodeIdentifierUpperCamel(Field field) => field.CsName ?? SyntaxHelpers.MakeUpperCamel(field.Name);
        string GetCodeIdentifierLowerCamel(Field field) => field.CsName ?? IdentifierRenamer.ToNonKeyword(SyntaxHelpers.MakeLowerCamel(field.Name));

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
                return new Name(GetCodeIdentifierLowerCamel(field));
            }

            var typeNames = new HashSet<Name>(def.NestedTypes.Select(t => MakeTypeName(t)));
            typeNames.Add(MakeTypeName(def));

            foreach (var member in def.Fields)
            {
                var memberName = new Name(GetCodeIdentifierUpperCamel(member));

                while (typeNames.Contains(memberName))
                {
                    memberName = new Name(string.Format(PropertyNamedLikeTypeRenameFormat, memberName.ToString()));
                }

                _fieldNameMap.Add(member, memberName);
            }

            return _fieldNameMap[field];
        }

        public string GetCodeIdentifier(TypeDefinition def)
        {
            string id = def.CsName ?? def.Name;
            if (def.Tag == TypeTag.Group) // special treatment for groups: Need to disambiguate between 
            {  // the field name (use original name) and its type (make it start with a lower-case letter)
                id = IdentifierRenamer.ToNonKeyword(SyntaxHelpers.MakeLowerCamel(id));
            }
            return id;
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

        public TypeSyntax MakeNullableRefType(TypeSyntax type)
        {
            return NullableEnable ?
                NullableType(type) :
                type;

        }

        public TypeSyntax Type<T>(Nullability nullability)
        {
            return (NullableEnable && !typeof(T).IsValueType && nullability != Nullability.NonNullable) ||
                   (                   typeof(T).IsValueType && nullability == Nullability.NullableRefAndValue) ?
                NullableType(SyntaxHelpers.NonNullableType<T>()) :
                SyntaxHelpers.NonNullableType<T>();                
        }

        public ExpressionSyntax SuppressNullableWarning(ExpressionSyntax expression)
        {
            return NullableEnable ?
                PostfixUnaryExpression(SyntaxKind.SuppressNullableWarningExpression, expression) :
                expression;
        }

        public SyntaxToken TypeVisibilityModifier
        {
            get
            {
                switch (TypeVisibility)
                {
                    case SupportedAnnotations.TypeVisibility.Public:
                        return Token(SyntaxKind.PublicKeyword);

                    case SupportedAnnotations.TypeVisibility.Internal:
                        return Token(SyntaxKind.InternalKeyword);

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        static LiteralExpressionSyntax HexLiteral(ulong id) =>
            LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                Literal($"0x{id:x}UL", id));

        static LiteralExpressionSyntax StringLiteral(string text) =>
            LiteralExpression(
                SyntaxKind.StringLiteralExpression,
                Literal(text));

        public FieldDeclarationSyntax MakeTypeIdConst(ulong id) =>
            FieldDeclaration(
                VariableDeclaration(
                    IdentifierName("UInt64"))
                .WithVariables(
                    SingletonSeparatedList<VariableDeclaratorSyntax>(
                        VariableDeclarator(TypeIdField.Identifier)
                        .WithInitializer(
                            EqualsValueClause(HexLiteral(id))))))
                .WithModifiers(
                    TokenList(
                        new[]{
                            Token(SyntaxKind.PublicKeyword),
                            Token(SyntaxKind.ConstKeyword)}));

        static AttributeSyntax MakeTypeIdAttribute(ulong id) =>
            Attribute(
                IdentifierName("TypeId"))
            .WithArgumentList(
                AttributeArgumentList(
                    SingletonSeparatedList<AttributeArgumentSyntax>(
                        AttributeArgument(HexLiteral(id)))));

        public AttributeSyntax MakeGeneratedCodeAttribute() =>
            Attribute(
                IdentifierName("System.CodeDom.Compiler.GeneratedCode"))
            .AddArgumentListArguments(
                AttributeArgument(StringLiteral(GeneratorToolName)),
                AttributeArgument(StringLiteral(GeneratorToolVersion)));

        public AttributeListSyntax MakeTypeDecorationAttributes(ulong typeId) =>
            AttributeList().AddAttributes(MakeGeneratedCodeAttribute(), MakeTypeIdAttribute(typeId));
    }
}
