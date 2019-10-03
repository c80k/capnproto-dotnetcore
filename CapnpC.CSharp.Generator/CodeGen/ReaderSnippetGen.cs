using System;
using System.Collections.Generic;
using System.Linq;
using CapnpC.CSharp.Generator.Model;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static CapnpC.CSharp.Generator.CodeGen.SyntaxHelpers;

namespace CapnpC.CSharp.Generator.CodeGen
{
    class ReaderSnippetGen
    {
        readonly GenNames _names;

        public ReaderSnippetGen(GenNames names)
        {
            _names = names;
        }

        MemberDeclarationSyntax MakeReaderImplicitConversionOperator1()
        {
            return ConversionOperatorDeclaration(
                    Token(SyntaxKind.ImplicitKeyword),
                    IdentifierName(nameof(Capnp.DeserializerState)))
                .WithModifiers(
                    TokenList(
                        new[]{
                            Token(SyntaxKind.PublicKeyword),
                            Token(SyntaxKind.StaticKeyword)}))
                .WithParameterList(
                    ParameterList(
                        SingletonSeparatedList<ParameterSyntax>(
                            Parameter(_names.ReaderParameter.Identifier)
                            .WithType(_names.ReaderStruct.IdentifierName))))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            _names.ReaderParameter.IdentifierName,
                            _names.ReaderContextField.IdentifierName)))
                .WithSemicolonToken(
                    Token(SyntaxKind.SemicolonToken));
        }

        MemberDeclarationSyntax MakeReaderImplicitConversionOperator2()
        {
            return ConversionOperatorDeclaration(
                Token(SyntaxKind.ImplicitKeyword),
                _names.ReaderStruct.IdentifierName)
                .WithModifiers(
                    TokenList(
                        new[] {
                            Token(SyntaxKind.PublicKeyword),
                            Token(SyntaxKind.StaticKeyword) }))
                .WithParameterList(
                    ParameterList(
                        SingletonSeparatedList<ParameterSyntax>(
                            Parameter(_names.ReaderContextField.Identifier)
                            .WithType(Type<Capnp.DeserializerState>()))))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        ObjectCreationExpression(_names.ReaderStruct.IdentifierName)
                        .WithArgumentList(
                            ArgumentList(
                                SingletonSeparatedList<ArgumentSyntax>(
                                    Argument(_names.ReaderContextField.IdentifierName))))))
                .WithSemicolonToken(
                    Token(SyntaxKind.SemicolonToken));
        }

        MemberDeclarationSyntax MakeReaderCreateMethod()
        {
            return MethodDeclaration(_names.ReaderStruct.IdentifierName, _names.ReaderCreateMethod.Identifier)
                .AddModifiers(Public, Static)
                .WithParameterList(
                    ParameterList(
                        SingletonSeparatedList(
                            Parameter(_names.ContextParameter.Identifier)
                            .WithType(
                                Type<Capnp.DeserializerState>()))))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        ObjectCreationExpression(_names.ReaderStruct.IdentifierName)
                        .AddArgumentListArguments(Argument(_names.ContextParameter.IdentifierName))))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        }

        IEnumerable<MemberDeclarationSyntax> MakeReaderStructMembers()
        {
            yield return FieldDeclaration(
                VariableDeclaration(
                    Type<Capnp.DeserializerState>())
                .AddVariables(_names.ReaderContextField.VariableDeclarator))
                .AddModifiers(Readonly);

            yield return ConstructorDeclaration(_names.ReaderStruct.Identifier)
                .AddModifiers(Public)
                .WithParameterList(
                    ParameterList(
                        SingletonSeparatedList(
                            Parameter(_names.ContextParameter.Identifier)
                            .WithType(Type<Capnp.DeserializerState>()))))
                .WithBody(
                    Block(
                        SingletonList<StatementSyntax>(
                            ExpressionStatement(
                                AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        ThisExpression(),
                                        _names.ReaderContextField.IdentifierName),
                                    _names.ContextParameter.IdentifierName)))));

            yield return MakeReaderCreateMethod();
            yield return MakeReaderImplicitConversionOperator1();
            yield return MakeReaderImplicitConversionOperator2();
        }

        IEnumerable<MemberDeclarationSyntax> MakeGroupReaderStructMembers()
        {
            yield return FieldDeclaration(
                VariableDeclaration(
                    Type<Capnp.DeserializerState>())
                .AddVariables(_names.ReaderContextField.VariableDeclarator))
                .AddModifiers(Readonly);

            yield return ConstructorDeclaration(_names.ReaderStruct.Identifier)
                .AddModifiers(Public)
                .WithParameterList(
                    ParameterList(
                        SingletonSeparatedList(Parameter(_names.GroupReaderContextArg.Identifier)
                            .WithType(Type<Capnp.DeserializerState>()))))
                .WithBody(
                    Block(
                        SingletonList<StatementSyntax>(
                            ExpressionStatement(
                                AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        ThisExpression(),
                                        _names.ReaderContextField.IdentifierName),
                                    _names.GroupReaderContextArg.IdentifierName)))));

            yield return MakeReaderCreateMethod();
            yield return MakeReaderImplicitConversionOperator1();
            yield return MakeReaderImplicitConversionOperator2();
        }

        PropertyDeclarationSyntax MakeReaderProperty(TypeSyntax type, string name, ExpressionSyntax right, bool cond)
        {
            if (cond)
            {
                right = ConditionalExpression(
                    BinaryExpression(
                        SyntaxKind.EqualsExpression,
                        _names.UnionDiscriminatorProp.IdentifierName,
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            _names.UnionDiscriminatorEnum.IdentifierName,
                            IdentifierName(name))),
                    right,
                    LiteralExpression(
                        SyntaxKind.DefaultLiteralExpression,
                        Token(SyntaxKind.DefaultKeyword)));
            }

            return PropertyDeclaration(type, name)
                .AddModifiers(Public)
                .WithExpressionBody(ArrowExpressionClause(right))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        }

        static Func<ExpressionSyntax, ExpressionSyntax> MakeCastFunc(TypeSyntax type) => 
            x => CastExpression(type, x);

        static Func<ExpressionSyntax, ExpressionSyntax> MakeListCastFunc(string castName, Field field)
        { 
            // Insight: List may have complex default values (e.g. [true, false, false, true] as a
            // valid default value for a list of bools. This does not yet fit the author's mindset.

            //if (field.DefaultValueIsExplicit)
            //{
            //    return x => InvocationExpression(
            //        MemberAccessExpression(
            //            SyntaxKind.SimpleMemberAccessExpression,
            //            x,
            //            IdentifierName(castName))
            //        )
            //        .AddArgumentListArguments(
            //            Argument(ValueOf(field.DefaultValue)));
            //}
            //else

            {
                return x => InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        x,
                        IdentifierName(castName))
                    );
            }
        }

        static Func<ExpressionSyntax, ExpressionSyntax> MakeListCastFuncWithCons(
            string castName, ExpressionSyntax cons)
        {
            return x => InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    x,
                    IdentifierName(castName))
                ).AddArgumentListArguments(Argument(cons));
        }

        static Func<ExpressionSyntax, ExpressionSyntax> MakeGenericListCastFunc(string castName, TypeSyntax genericArgument)
        {
            return x => InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    x,
                    GenericName(castName).AddTypeArgumentListArguments(genericArgument))
                );
        }

        PropertyDeclarationSyntax MakeReadProperty(TypeSyntax type, string name, SimpleNameSyntax readName, 
            object indexOrBitOffset, ExpressionSyntax secondArg,
            Func<ExpressionSyntax, ExpressionSyntax> cast, bool cond)
        {
            var right = InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    _names.ReaderContextField.IdentifierName,
                    readName))
                    .AddArgumentListArguments(
                        Argument(ValueOf(indexOrBitOffset)));

            if (secondArg != null)
            {
                right = right.AddArgumentListArguments(Argument(secondArg));
            }

            ExpressionSyntax expr = right;

            if (cast != null)
            {
                expr = cast(expr);
            }

            return MakeReaderProperty(type, name, expr, cond);
        }

        PropertyDeclarationSyntax MakeReadProperty(TypeSyntax type, string name, string readName,
            object indexOrBitOffset, ExpressionSyntax secondArg,
            Func<ExpressionSyntax, ExpressionSyntax> cast, bool cond)
        {
            return MakeReadProperty(type, name, IdentifierName(readName), indexOrBitOffset, secondArg, cast, cond);
        }

        PropertyDeclarationSyntax MakeReadPrimitiveProperty<T>(Field field, string readName)
        {
            return MakeReadProperty(Type<T>(), _names.GetCodeIdentifier(field).ToString(), readName, field.BitOffset.Value,
                ValueOf(field.DefaultValue.ScalarValue), null, field.DiscValue.HasValue);
        }

        PropertyDeclarationSyntax MakeReadEnumProperty(Field field)
        {
            var typeSyntax = _names.MakeTypeSyntax(field.Type, field.DeclaringType, TypeUsage.Reader);
            return MakeReadProperty(typeSyntax, 
                _names.GetCodeIdentifier(field).ToString(), 
                nameof(Capnp.SerializerExtensions.ReadDataUShort), field.BitOffset.Value,
                ValueOf(field.DefaultValue.ScalarValue),
                x => CastExpression(typeSyntax, x), 
                field.DiscValue.HasValue);
        }

        PropertyDeclarationSyntax MakeReadTextProperty(Field field)
        {
            return MakeReadProperty(Type<string>(), _names.GetCodeIdentifier(field).ToString(),
                nameof(Capnp.DeserializerState.ReadText), (int)field.Offset,
                ValueOf(field.DefaultValue.ScalarValue), null, field.DiscValue.HasValue);
        }

        MemberAccessExpressionSyntax MakeReaderCreator(TypeSyntax qtype)
        {
            return MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                qtype,
                _names.ReaderCreateMethod.IdentifierName);
        }

        PropertyDeclarationSyntax MakeReadStructProperty(Field field)
        {
            var qtype = _names.MakeTypeSyntax(field.Type, field.DeclaringType, TypeUsage.Reader);
            var creator = MakeReaderCreator(qtype);

            return MakeReadProperty(qtype, _names.GetCodeIdentifier(field).ToString(),
                nameof(Capnp.DeserializerState.ReadStruct), (int)field.Offset,
                creator, null, field.DiscValue.HasValue);
        }

        PropertyDeclarationSyntax MakeReadGroupProperty(Field field)
        {
            var type = QualifiedName(
                _names.MakeTypeName(field.Type.Definition).IdentifierName,
                _names.ReaderStruct.IdentifierName);

            var right = ObjectCreationExpression(type)
                    .WithArgumentList(
                        ArgumentList(
                            SingletonSeparatedList(
                                Argument(_names.ReaderContextField.IdentifierName))));

            return MakeReaderProperty(type, _names.GetCodeIdentifier(field).ToString(), right, field.DiscValue.HasValue);
        }

        void MakeReadListPropertyImpl(Model.Type elementType, TypeDefinition scope, ExpressionSyntax context, int depth,
            out TypeSyntax listType, out ExpressionSyntax impl)
        {
            var elementTypeSyntax = _names.MakeTypeSyntax(elementType, scope, TypeUsage.Reader);
            listType = GenericName("IReadOnlyList").AddTypeArgumentListArguments(elementTypeSyntax);

            if (elementType.Tag == TypeTag.Interface ||
                elementType.Tag == TypeTag.CapabilityPointer)
            {
                if (depth == 0)
                {
                    impl = InvocationExpression(MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            _names.ReaderContextField.IdentifierName,
                            GenericName(nameof(Capnp.DeserializerState.ReadCapList))
                                .AddTypeArgumentListArguments(elementTypeSyntax)
                            )).AddArgumentListArguments(Argument(context));
                }
                else
                {
                    impl = InvocationExpression(
                        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                            context,
                            GenericName(nameof(Capnp.DeserializerState.RequireCapList))
                                .AddTypeArgumentListArguments(elementTypeSyntax)
                            ));
                }

                return;
            }

            if (depth == 0)
            {
                context = InvocationExpression(MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    _names.ReaderContextField.IdentifierName,
                    IdentifierName(nameof(Capnp.DeserializerState.ReadList))))
                    .AddArgumentListArguments(Argument(context));
            }
            else
            {
                context = InvocationExpression(
                    MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                        context,
                        IdentifierName(nameof(Capnp.DeserializerState.RequireList))
                        ));
            }

            string lambdaParamName = "_" + depth;
            var lambdaParam = Parameter(Identifier(lambdaParamName));
            var lambdaArg = IdentifierName(lambdaParamName);
            string castFuncName;

            switch (elementType.Tag)
            {
                case TypeTag.List:
                    {

                        MakeReadListPropertyImpl(
                            elementType.ElementType,
                            scope,
                            lambdaArg,
                            depth + 1,
                            out var innerListType,
                            out var innerImpl);

                        listType = GenericName("IReadOnlyList").AddTypeArgumentListArguments(innerListType);

                        impl = InvocationExpression(
                            MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                            context,
                            IdentifierName(nameof(Capnp.ListDeserializer.Cast))))
                                .AddArgumentListArguments(
                                    Argument(SimpleLambdaExpression(lambdaParam, innerImpl)));

                        return;
                    }

                case TypeTag.ListPointer:
                    {
                        listType = Type<IReadOnlyList<object>>();

                        context = InvocationExpression(MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            context,
                            IdentifierName(nameof(Capnp.DeserializerState.RequireList))
                            )).AddArgumentListArguments(Argument(lambdaArg));

                        impl = InvocationExpression(
                            MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                            context,
                            IdentifierName(nameof(Capnp.ReadOnlyListExtensions.LazyListSelect))))
                                .AddArgumentListArguments(
                                    Argument(SimpleLambdaExpression(lambdaParam, context)));

                        return;
                    }

                case TypeTag.Struct:
                    {
                        impl = InvocationExpression(
                            MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                            context,
                            IdentifierName(nameof(Capnp.ListDeserializer.Cast))))
                                .AddArgumentListArguments(
                                    Argument(MakeReaderCreator(elementTypeSyntax)));

                        return;
                    }

                case TypeTag.Enum:
                    {
                        var cons = SimpleLambdaExpression(
                            lambdaParam,
                            CastExpression(elementTypeSyntax, lambdaArg));

                        impl = InvocationExpression(
                            MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                            context,
                            IdentifierName(nameof(Capnp.ListDeserializer.CastEnums))))
                                .AddArgumentListArguments(
                                    Argument(cons));

                        return;
                    }

                case TypeTag.AnyPointer:
                case TypeTag.StructPointer:
                    {
                        listType = Type<IReadOnlyList<Capnp.DeserializerState>>();
                        impl = context;
                        return;
                    }

                case TypeTag.Void:
                    {
                        listType = Type<int>();
                        impl = MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            context,
                            IdentifierName(nameof(Capnp.ListDeserializer.Count)));
                        return;
                    }

                case TypeTag.Data:
                    castFuncName = nameof(Capnp.ListDeserializer.CastData);
                    break;

                case TypeTag.Text:
                    castFuncName = nameof(Capnp.ListDeserializer.CastText2);
                    break;

                case TypeTag.Bool:
                    castFuncName = nameof(Capnp.ListDeserializer.CastBool);
                    break;

                case TypeTag.F32:
                    castFuncName = nameof(Capnp.ListDeserializer.CastFloat);
                    break;

                case TypeTag.F64:
                    castFuncName = nameof(Capnp.ListDeserializer.CastDouble);
                    break;

                case TypeTag.S8:
                    castFuncName = nameof(Capnp.ListDeserializer.CastSByte);
                    break;

                case TypeTag.U8:
                    castFuncName = nameof(Capnp.ListDeserializer.CastByte);
                    break;

                case TypeTag.S16:
                    castFuncName = nameof(Capnp.ListDeserializer.CastShort);
                    break;

                case TypeTag.U16:
                case TypeTag.AnyEnum:
                    castFuncName = nameof(Capnp.ListDeserializer.CastUShort);
                    break;

                case TypeTag.S32:
                    castFuncName = nameof(Capnp.ListDeserializer.CastInt);
                    break;

                case TypeTag.U32:
                    castFuncName = nameof(Capnp.ListDeserializer.CastUInt);
                    break;

                case TypeTag.S64:
                    castFuncName = nameof(Capnp.ListDeserializer.CastLong);
                    break;

                case TypeTag.U64:
                    castFuncName = nameof(Capnp.ListDeserializer.CastULong);
                    break;

                default:
                    throw new NotImplementedException("Unexpected type tag, don't know how to deal with this");
            }

            impl = InvocationExpression(MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                context,
                IdentifierName(castFuncName)));
        }

        PropertyDeclarationSyntax MakeReadListProperty(Field field)
        {
            var elementType = field.Type.ElementType;
            var context = ValueOf((int)field.Offset);
            MakeReadListPropertyImpl(elementType, field.DeclaringType, context, 0, out var listType, out var impl);
            return MakeReaderProperty(listType, _names.GetCodeIdentifier(field).ToString(), impl, field.DiscValue.HasValue);
        }

        PropertyDeclarationSyntax MakeReadAnyListProperty(Field field)
        {
            var type = _names.MakeTypeSyntax(field.Type, field.DeclaringType, TypeUsage.Reader);

            return MakeReadProperty(type, _names.GetCodeIdentifier(field).ToString(),
                nameof(Capnp.DeserializerState.ReadList),
                (int)field.Offset, null, x => CastExpression(type, x), field.DiscValue.HasValue);
        }

        PropertyDeclarationSyntax MakeReadCapProperty(Field field)
        {
            var type = _names.MakeTypeSyntax(field.Type, field.DeclaringType, TypeUsage.Reader);
            var readName = GenericName(nameof(Capnp.DeserializerState.ReadCap))
                .AddTypeArgumentListArguments(type);

            return MakeReadProperty(type, _names.GetCodeIdentifier(field).ToString(),
                readName,
                (int)field.Offset, null, null, field.DiscValue.HasValue);
        }

        PropertyDeclarationSyntax MakeReadAnyCapProperty(Field field)
        {
            var type = _names.MakeTypeSyntax(field.Type, field.DeclaringType, TypeUsage.Reader);

            return MakeReadProperty(type, _names.GetCodeIdentifier(field).ToString(),
                nameof(Capnp.DeserializerState.ReadCap),
                (int)field.Offset, null, null, field.DiscValue.HasValue);
        }

        PropertyDeclarationSyntax MakeReadDataProperty(Field field)
        {
            var context = InvocationExpression(MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                _names.ReaderContextField.IdentifierName,
                IdentifierName(nameof(Capnp.DeserializerState.ReadList))))
                .AddArgumentListArguments(Argument(ValueOf((int)field.Offset)));
            var impl = InvocationExpression(MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                context,
                IdentifierName(nameof(Capnp.ListDeserializer.CastByte))));

            return MakeReaderProperty(
                _names.MakeTypeSyntax(field.Type, field.DeclaringType, TypeUsage.Reader), 
                _names.GetCodeIdentifier(field).ToString(), impl, field.DiscValue.HasValue);
        }

        PropertyDeclarationSyntax MakeReadAnyPointerProperty(Field field)
        {
            var type = IdentifierName(nameof(Capnp.DeserializerState));

            return MakeReadProperty(type, _names.GetCodeIdentifier(field).ToString(),
                nameof(Capnp.DeserializerState.StructReadPointer), 
                (int)field.Offset, null, null, field.DiscValue.HasValue);
        }

        PropertyDeclarationSyntax MakeReaderUnionSelector(TypeDefinition def)
        {
            var type = _names.UnionDiscriminatorEnum.IdentifierName;
            return MakeReadProperty(
                type,
                _names.UnionDiscriminatorProp.ToString(),
                nameof(Capnp.SerializerExtensions.ReadDataUShort),
                def.UnionInfo.TagOffset,
                ValueOf(default(ushort)),
                MakeCastFunc(type), false);
        }

        PropertyDeclarationSyntax MakeReaderFieldProperty(Field field)
        {
            switch (field.Type.Tag)
            {
                case TypeTag.Bool:
                    return MakeReadPrimitiveProperty<bool>(field,
                        nameof(Capnp.SerializerExtensions.ReadDataBool));

                case TypeTag.S8:
                    return MakeReadPrimitiveProperty<sbyte>(field,
                        nameof(Capnp.SerializerExtensions.ReadDataSByte));

                case TypeTag.U8:
                    return MakeReadPrimitiveProperty<byte>(field,
                        nameof(Capnp.SerializerExtensions.ReadDataByte));

                case TypeTag.S16:
                    return MakeReadPrimitiveProperty<short>(field,
                        nameof(Capnp.SerializerExtensions.ReadDataShort));

                case TypeTag.U16:
                    return MakeReadPrimitiveProperty<ushort>(field,
                        nameof(Capnp.SerializerExtensions.ReadDataUShort));

                case TypeTag.S32:
                    return MakeReadPrimitiveProperty<int>(field,
                        nameof(Capnp.SerializerExtensions.ReadDataInt));

                case TypeTag.U32:
                    return MakeReadPrimitiveProperty<uint>(field,
                        nameof(Capnp.SerializerExtensions.ReadDataUInt));

                case TypeTag.S64:
                    return MakeReadPrimitiveProperty<long>(field,
                        nameof(Capnp.SerializerExtensions.ReadDataLong));

                case TypeTag.U64:
                    return MakeReadPrimitiveProperty<ulong>(field,
                        nameof(Capnp.SerializerExtensions.ReadDataULong));

                case TypeTag.F32:
                    return MakeReadPrimitiveProperty<float>(field,
                        nameof(Capnp.SerializerExtensions.ReadDataFloat));

                case TypeTag.F64:
                    return MakeReadPrimitiveProperty<double>(field,
                        nameof(Capnp.SerializerExtensions.ReadDataDouble));

                case TypeTag.Enum:
                    return MakeReadEnumProperty(field);

                case TypeTag.Text:
                    return MakeReadTextProperty(field);

                case TypeTag.Struct:
                    return MakeReadStructProperty(field);

                case TypeTag.Group:
                    return MakeReadGroupProperty(field);

                case TypeTag.List:
                    return MakeReadListProperty(field);

                case TypeTag.Interface:
                    return MakeReadCapProperty(field);

                case TypeTag.CapabilityPointer:
                    return MakeReadAnyCapProperty(field);

                case TypeTag.ListPointer:
                    return MakeReadAnyListProperty(field);

                case TypeTag.AnyPointer:
                case TypeTag.StructPointer:
                    return MakeReadAnyPointerProperty(field);

                case TypeTag.Data:
                    return MakeReadDataProperty(field);

                default:
                    return null;
            }
        }

        public StructDeclarationSyntax MakeReaderStruct(TypeDefinition def)
        {
            var readerDecl = StructDeclaration(_names.ReaderStruct.ToString()).AddModifiers(Public);

            var members = def.Tag == TypeTag.Group ? 
                MakeGroupReaderStructMembers() :
                MakeReaderStructMembers();

            readerDecl = readerDecl.AddMembers(members.ToArray());

            if (def.UnionInfo != null)
            {
                readerDecl = readerDecl.AddMembers(MakeReaderUnionSelector(def));
            }

            foreach (var field in def.Fields)
            {
                var propDecl = MakeReaderFieldProperty(field);

                if (propDecl != null)
                {
                    readerDecl = readerDecl.AddMembers(propDecl);
                }
            }

            return readerDecl;
        }
    }
}
