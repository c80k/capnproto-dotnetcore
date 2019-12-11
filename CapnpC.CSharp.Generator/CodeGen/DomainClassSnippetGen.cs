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
    class DomainClassSnippetGen
    {
        readonly GenNames _names;

        public DomainClassSnippetGen(GenNames names)
        {
            _names = names;
        }

        MemberDeclarationSyntax MakeUnionField(Field field)
        {
            var type = _names.MakeTypeSyntax(field.Type, field.DeclaringType, TypeUsage.DomainClassNullable);

            switch (field.Type.Tag)
            {
                case TypeTag.Void:
                    return null;

                default:
                    return PropertyDeclaration(type,
                        _names.GetCodeIdentifier(field).Identifier)
                        .AddModifiers(Public).AddAccessorListAccessors(
                            AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                .WithExpressionBody(
                                    ArrowExpressionClause(
                                        ConditionalExpression(
                                            BinaryExpression(
                                                SyntaxKind.EqualsExpression,
                                                _names.UnionDiscriminatorField.IdentifierName,
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    _names.UnionDiscriminatorEnum.IdentifierName,
                                                    _names.GetCodeIdentifier(field).IdentifierName)),
                                            CastExpression(type,
                                                _names.UnionContentField.IdentifierName),
                                            LiteralExpression(
                                                SyntaxKind.NullLiteralExpression))))
                                .WithSemicolonToken(
                                    Token(SyntaxKind.SemicolonToken)),
                                AccessorDeclaration(
                                    SyntaxKind.SetAccessorDeclaration)
                                .WithBody(
                                    Block(
                                        ExpressionStatement(
                                            AssignmentExpression(
                                                SyntaxKind.SimpleAssignmentExpression,
                                                _names.UnionDiscriminatorField.IdentifierName,
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    _names.UnionDiscriminatorEnum.IdentifierName,
                                                    _names.GetCodeIdentifier(field).IdentifierName))),
                                        ExpressionStatement(
                                            AssignmentExpression(
                                                SyntaxKind.SimpleAssignmentExpression,
                                                _names.UnionContentField.IdentifierName,
                                                IdentifierName("value"))))));
            }
        }

        MemberDeclarationSyntax MakeStructField(Field field)
        {
            if (field.Type.Tag == TypeTag.Void)
            {
                return null;
            }

            var prop = PropertyDeclaration(_names.MakeTypeSyntax(field.Type, field.DeclaringType, TypeUsage.DomainClass),
                _names.GetCodeIdentifier(field).Identifier)
                .AddModifiers(Public).AddAccessorListAccessors(
                    AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                    AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));

            if (field.DefaultValueIsExplicit && field.Type.IsValueType)
            {
                prop = prop.WithInitializer(
                    EqualsValueClause(MakeDefaultValue(field)))
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
            }

            return prop;
        }

        MemberDeclarationSyntax MakeUnionDiscriminatorField()
        {
            return FieldDeclaration(
                VariableDeclaration(_names.UnionDiscriminatorEnum.IdentifierName)
                .AddVariables(
                    VariableDeclarator(_names.UnionDiscriminatorField.Identifier)
                        .WithInitializer(
                            EqualsValueClause(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    _names.UnionDiscriminatorEnum.IdentifierName,
                                    _names.UnionDiscriminatorUndefined.IdentifierName)))))
                .AddModifiers(Private);
        }

        MemberDeclarationSyntax MakeUnionContentField()
        {
            return FieldDeclaration(
                VariableDeclaration(SyntaxHelpers.Type<object>())
                .WithVariables(
                    SingletonSeparatedList<VariableDeclaratorSyntax>(
                        VariableDeclarator(_names.UnionContentField.Identifier))))
                .AddModifiers(Private);
        }

        IEnumerable<ExpressionSyntax> MakeInitializerAssignments(Value structValue, TypeDefinition scope)
        {
            foreach (var fieldValue in structValue.Fields)
            {
                var valueExpr = MakeValue(fieldValue.Item2, scope);
                if (valueExpr == null)
                    continue;

                yield return AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    _names.GetCodeIdentifier(fieldValue.Item1).IdentifierName,
                    valueExpr);
            }
        }

        ExpressionSyntax MakeValue(Value value, TypeDefinition scope)
        {
            switch (value.Type.Tag)
            {
                case TypeTag.AnyEnum:
                    return LiteralExpression(
                        SyntaxKind.NumericLiteralExpression, Literal((ushort)value.ScalarValue));

                case TypeTag.Bool:

                    if ((bool)value.ScalarValue)
                        return LiteralExpression(SyntaxKind.TrueLiteralExpression);
                    else
                        return LiteralExpression(SyntaxKind.FalseLiteralExpression);

                case TypeTag.Data:
                    return ArrayCreationExpression(ArrayType(
                        PredefinedType(Token(SyntaxKind.ByteKeyword)))
                        .WithRankSpecifiers(
                            SingletonList<ArrayRankSpecifierSyntax>(
                                ArrayRankSpecifier(
                                    SingletonSeparatedList<ExpressionSyntax>(
                                        OmittedArraySizeExpression())))))
                        .WithInitializer(
                            InitializerExpression(
                                SyntaxKind.ArrayInitializerExpression)
                                .AddExpressions(value.Items.Select(v => MakeValue(v, scope)).ToArray()));

                case TypeTag.Enum:
                    return MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                        _names.MakeTypeSyntax(value.Type, scope, TypeUsage.NotRelevant),
                        IdentifierName(value.GetEnumerant().Literal));

                case TypeTag.F32:
                    switch ((float)value.ScalarValue)
                    {
                        case float.NaN:
                            return MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression, 
                                IdentifierName("float"), 
                                IdentifierName(nameof(float.NaN)));

                        case float.NegativeInfinity:
                            return MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression, 
                                IdentifierName("float"), 
                                IdentifierName(nameof(float.NegativeInfinity)));

                        case float.PositiveInfinity:
                            return MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("float"),
                                IdentifierName(nameof(float.PositiveInfinity)));

                        default:
                            return LiteralExpression(SyntaxKind.NumericLiteralExpression,
                                Literal((float)value.ScalarValue));
                    }

                case TypeTag.F64:
                    switch ((double)value.ScalarValue)
                    {
                        case double.NaN:
                            return MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("double"),
                                IdentifierName(nameof(double.NaN)));

                        case double.NegativeInfinity:
                            return MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("double"),
                                IdentifierName(nameof(double.NegativeInfinity)));

                        case double.PositiveInfinity:
                            return MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("double"),
                                IdentifierName(nameof(double.PositiveInfinity)));

                        default:
                            return LiteralExpression(SyntaxKind.NumericLiteralExpression,
                                Literal((double)value.ScalarValue));
                    }

                case TypeTag.S8:
                    return LiteralExpression(SyntaxKind.NumericLiteralExpression,
                        Literal((sbyte)value.ScalarValue));

                case TypeTag.S16:
                    return LiteralExpression(SyntaxKind.NumericLiteralExpression,
                        Literal((short)value.ScalarValue));

                case TypeTag.S32:
                    return LiteralExpression(SyntaxKind.NumericLiteralExpression,
                        Literal((int)value.ScalarValue));

                case TypeTag.S64:
                    return LiteralExpression(SyntaxKind.NumericLiteralExpression,
                        Literal((long)value.ScalarValue));

                case TypeTag.U8:
                    return LiteralExpression(SyntaxKind.NumericLiteralExpression,
                        Literal((byte)value.ScalarValue));

                case TypeTag.U16:
                    return LiteralExpression(SyntaxKind.NumericLiteralExpression,
                        Literal((ushort)value.ScalarValue));

                case TypeTag.U32:
                    return LiteralExpression(SyntaxKind.NumericLiteralExpression,
                        Literal((uint)value.ScalarValue));

                case TypeTag.U64:
                    return LiteralExpression(SyntaxKind.NumericLiteralExpression,
                        Literal((ulong)value.ScalarValue));

                case TypeTag.Text:
                    value.Decode();
                    return value.ScalarValue == null ?
                        LiteralExpression(SyntaxKind.NullLiteralExpression) :
                        LiteralExpression(SyntaxKind.StringLiteralExpression,
                            Literal((string)value.ScalarValue));

                case TypeTag.Group:
                case TypeTag.Struct:
                    value.Decode();

                    return ObjectCreationExpression(
                        _names.MakeTypeSyntax(value.Type, scope, TypeUsage.DomainClass))
                        .WithArgumentList(ArgumentList())
                        .WithInitializer(
                            InitializerExpression(
                                SyntaxKind.ObjectInitializerExpression)
                                .AddExpressions(MakeInitializerAssignments(value, scope).ToArray()));

                case TypeTag.ListPointer:
                    // TBD
                    return LiteralExpression(SyntaxKind.NullLiteralExpression);

                case TypeTag.List when value.Type.ElementType.Tag == TypeTag.Void:
                    value.Decode();

                    return LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal((int)value.VoidListCount));

                case TypeTag.List:
                    value.Decode();

                    return ArrayCreationExpression(ArrayType(
                        _names.MakeTypeSyntax(value.Type.ElementType, scope, TypeUsage.DomainClass))
                        .WithRankSpecifiers(
                            SingletonList<ArrayRankSpecifierSyntax>(
                                ArrayRankSpecifier(
                                    SingletonSeparatedList<ExpressionSyntax>(
                                        OmittedArraySizeExpression())))))
                        .WithInitializer(
                            InitializerExpression(
                                SyntaxKind.ArrayInitializerExpression)
                                .AddExpressions(value.Items.Select(v => MakeValue(v, scope)).ToArray()));

                case TypeTag.AnyPointer:
                case TypeTag.CapabilityPointer:
                    // TBD
                    return null;

                case TypeTag.Interface:
                    return null;

                default:
                    throw new NotImplementedException();
            }
        }

        ExpressionSyntax MakeDefaultValue(Field field)
        {
            if (field.DefaultValueIsExplicit)
            {
                return MakeValue(field.DefaultValue, field.DeclaringType);
            }
            else
            {
                switch (field.Type.Tag)
                {
                    case TypeTag.AnyEnum:
                    case TypeTag.S16:
                    case TypeTag.S32:
                    case TypeTag.S64:
                    case TypeTag.S8:
                    case TypeTag.U16:
                    case TypeTag.U32:
                    case TypeTag.U64:
                    case TypeTag.U8:
                        return LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(0));

                    case TypeTag.AnyPointer:
                    case TypeTag.CapabilityPointer:
                    case TypeTag.Data:
                    case TypeTag.Group:
                    case TypeTag.Interface:
                    case TypeTag.List:
                    case TypeTag.ListPointer:
                    case TypeTag.Struct:
                    case TypeTag.StructPointer:
                    case TypeTag.Text:
                        return LiteralExpression(SyntaxKind.NullLiteralExpression);

                    case TypeTag.Bool:
                        return LiteralExpression(SyntaxKind.FalseLiteralExpression);

                    case TypeTag.Enum:
                        return CastExpression(
                            _names.MakeTypeSyntax(field.Type, field.DeclaringType, TypeUsage.NotRelevant),
                            LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(0)));

                    case TypeTag.F32:
                        return LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(0.0f));

                    case TypeTag.F64:
                        return LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(0.0));

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        IEnumerable<SwitchSectionSyntax> MakeUnionDiscriminatorSetter(TypeDefinition def)
        {
            var unionFields = def.Fields.Where(f => f.DiscValue.HasValue);

            foreach (var unionField in unionFields)
            {
                var section = SwitchSection()
                    .WithLabels(
                        SingletonList<SwitchLabelSyntax>(
                            CaseSwitchLabel(MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                _names.UnionDiscriminatorEnum.IdentifierName,
                                _names.GetCodeIdentifier(unionField).IdentifierName))));

                if (unionField.Type.Tag != TypeTag.Void)
                {
                    section = section.AddStatements(
                        ExpressionStatement(
                                AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    _names.UnionContentField.IdentifierName,
                                    MakeDefaultValue(unionField))));
                }

                section = section.AddStatements(BreakStatement());

                yield return section;
            }
        }

        MemberDeclarationSyntax MakeUnionDiscriminatorProperty(TypeDefinition def)
        {
            return PropertyDeclaration(_names.UnionDiscriminatorEnum.IdentifierName,
                _names.UnionDiscriminatorProp.Identifier)
                .AddModifiers(Public).AddAccessorListAccessors(
                    AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithExpressionBody(
                            ArrowExpressionClause(_names.UnionDiscriminatorField.IdentifierName))
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                    AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                        .WithBody(
                            Block(
                                IfStatement(
                                    BinaryExpression(
                                        SyntaxKind.EqualsExpression,
                                        IdentifierName("value"),
                                        _names.UnionDiscriminatorField.IdentifierName),
                                    ReturnStatement()),
                                ExpressionStatement(
                                    AssignmentExpression(
                                        SyntaxKind.SimpleAssignmentExpression,
                                        _names.UnionDiscriminatorField.IdentifierName,
                                        IdentifierName("value"))),
                                SwitchStatement(IdentifierName("value"))
                                .WithOpenParenToken(
                                    Token(SyntaxKind.OpenParenToken))
                                .WithCloseParenToken(
                                    Token(SyntaxKind.CloseParenToken))
                                .AddSections(MakeUnionDiscriminatorSetter(def).ToArray()))));
        }

        MemberDeclarationSyntax MakeField(Field field)
        {
            if (field.DiscValue.HasValue)
                return MakeUnionField(field);
            else
                return MakeStructField(field);
        }

        ExpressionSyntax MakeListSerializeParticle(Model.Type type, ExpressionSyntax writer, ExpressionSyntax domain)
        {
            string s = $"_s{type.GetRank().Item1}";
            string v = $"_v{type.GetRank().Item1}";

            switch (type.ElementType?.Tag)
            {
                case TypeTag.List:
                case TypeTag.ListPointer:
                case TypeTag.Struct:
                case TypeTag.Group:
                case TypeTag.StructPointer:
                case TypeTag.Data:

                    return InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            writer,
                            IdentifierName(nameof(Capnp.ListOfPrimitivesSerializer<int>.Init))))
                            .AddArgumentListArguments(
                                Argument(domain),
                                Argument(
                                    ParenthesizedLambdaExpression(
                                        MakeComplexSerializeParticle(
                                            type.ElementType,
                                            IdentifierName(s),
                                            IdentifierName(v)))
                                        .AddParameterListParameters(
                                            Parameter(Identifier(s)),
                                            Parameter(Identifier(v)))));

                default:
                    return InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            writer,
                            IdentifierName(nameof(Capnp.ListOfPrimitivesSerializer<int>.Init))))
                            .AddArgumentListArguments(Argument(domain));
            }
        }

        ExpressionSyntax MakeComplexSerializeParticle(Model.Type type, ExpressionSyntax writer, ExpressionSyntax domain)
        {
            switch (type.Tag)
            {
                case TypeTag.Data:
                case TypeTag.List:
                    return MakeListSerializeParticle(type, writer, domain);

                case TypeTag.Struct:
                case TypeTag.Group:
                    return ConditionalAccessExpression(domain,
                        InvocationExpression(MemberBindingExpression(_names.SerializeMethod.IdentifierName))
                        .AddArgumentListArguments(Argument(writer)));

                default:
                    throw new NotImplementedException();
            }
        }

        StatementSyntax MakeSerializeMethodFieldAssignment(Field field)
        {
            var writerProp = MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        _names.WriterParameter.IdentifierName,
                                        _names.GetCodeIdentifier(field).IdentifierName);

            switch (field.Type.Tag)
            {
                case TypeTag.Bool:
                case TypeTag.Enum:
                case TypeTag.F32:
                case TypeTag.F64:
                case TypeTag.S16:
                case TypeTag.S32:
                case TypeTag.S64:
                case TypeTag.S8:
                case TypeTag.U16:
                case TypeTag.U32:
                case TypeTag.U64:
                case TypeTag.U8:
                case TypeTag.AnyEnum:
                case TypeTag.List when field.Type.Tag == TypeTag.Void:
                    if (field.DiscValue.HasValue)
                    {
                        return ExpressionStatement(
                                    AssignmentExpression(
                                        SyntaxKind.SimpleAssignmentExpression,
                                        writerProp,
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            _names.GetCodeIdentifier(field).IdentifierName,
                                            IdentifierName(nameof(Nullable<int>.Value)))));
                    }
                    else
                    {
                        return ExpressionStatement(
                                    AssignmentExpression(
                                        SyntaxKind.SimpleAssignmentExpression,
                                        writerProp,
                                        _names.GetCodeIdentifier(field).IdentifierName));
                    }

                case TypeTag.AnyPointer:
                case TypeTag.ListPointer:
                case TypeTag.StructPointer:
                    return ExpressionStatement(
                        InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    _names.WriterParameter.IdentifierName,
                                    _names.GetCodeIdentifier(field).IdentifierName),
                                IdentifierName(nameof(Capnp.DynamicSerializerState.SetObject))))
                        .AddArgumentListArguments(
                            Argument(_names.GetCodeIdentifier(field).IdentifierName)));

                case TypeTag.CapabilityPointer:
                case TypeTag.Interface:
                    return ExpressionStatement(
                                AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    writerProp,
                                    _names.GetCodeIdentifier(field).IdentifierName));

                case TypeTag.Text:
                    return ExpressionStatement(
                                AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    writerProp,
                                    _names.GetCodeIdentifier(field).IdentifierName));

                case TypeTag.Data:
                case TypeTag.List:
                case TypeTag.Struct:
                case TypeTag.Group:
                    return ExpressionStatement(
                        MakeComplexSerializeParticle(
                            field.Type, 
                            writerProp, 
                            _names.GetCodeIdentifier(field).IdentifierName));

                case TypeTag.Void:
                    return null;

                default:
                    throw new NotImplementedException();
            }
        }

        StatementSyntax MakeApplyDefaultsMethodFieldAssignment(Field field)
        {
            var lhs = _names.GetCodeIdentifier(field).IdentifierName;
            var rhs = MakeDefaultValue(field);

            if (rhs == null)
            {
                return null;
            }

            return ExpressionStatement(
                AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    lhs, 
                    BinaryExpression(SyntaxKind.CoalesceExpression,
                        lhs, rhs)));
        }

        ExpressionSyntax MakeInnerStructListConversion(ExpressionSyntax context, TypeSyntax elementType)
        {
            return InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    context,
                    IdentifierName(nameof(Capnp.ReadOnlyListExtensions.ToReadOnlyList))))
                    .AddArgumentListArguments(Argument(
                        SimpleLambdaExpression(Parameter(Identifier("_")),
                            InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName(nameof(Capnp.CapnpSerializable)),
                                    GenericName(nameof(Capnp.CapnpSerializable.Create))
                                        .AddTypeArgumentListArguments(elementType)))
                                .AddArgumentListArguments(Argument(IdentifierName("_"))))));
        }

        ExpressionSyntax MakeStructListConversion(ExpressionSyntax context, TypeSyntax elementType, int rank)
        {
            if (rank == 1)
            {
                return MakeInnerStructListConversion(context, elementType);
            }

            string lambdaVarName = $"_{rank}";

            return InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    context,
                    IdentifierName(nameof(Capnp.ReadOnlyListExtensions.ToReadOnlyList))))
                    .AddArgumentListArguments(Argument(
                        SimpleLambdaExpression(
                            Parameter(Identifier(lambdaVarName)),
                            MakeStructListConversion(IdentifierName(lambdaVarName), elementType, rank - 1))));
        }

        ExpressionSyntax MakeAnyListConversion(ExpressionSyntax context)
        {
            return InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    context,
                    IdentifierName(nameof(Capnp.ReadOnlyListExtensions.ToReadOnlyList))))
                    .AddArgumentListArguments(Argument(
                        SimpleLambdaExpression(
                            Parameter(Identifier("_")),
                            CastExpression(Type<object>(), IdentifierName("_")))));
        }

        ExpressionSyntax MakeDeserializeMethodRightHandSide(Field field)
        {
            switch (field.Type.Tag)
            {
                case TypeTag.Struct:
                case TypeTag.Group:
                case TypeTag.StructPointer:
                case TypeTag.AnyPointer:
                    return InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName(nameof(Capnp.CapnpSerializable)),
                            GenericName(nameof(Capnp.CapnpSerializable.Create))
                                .AddTypeArgumentListArguments(
                                    _names.MakeTypeSyntax(
                                        field.Type, 
                                        field.DeclaringType, 
                                        TypeUsage.DomainClass))))
                            .AddArgumentListArguments(Argument(MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression, 
                                _names.ReaderParameter.IdentifierName,
                                _names.GetCodeIdentifier(field).IdentifierName)));

                case TypeTag.Void:
                    return null;

                case TypeTag.List:
                    (var rank, var elementType) = field.Type.GetRank();
                    if (elementType.Tag != TypeTag.Struct)
                        break;

                    return MakeStructListConversion(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    _names.ReaderParameter.IdentifierName,
                                    _names.GetCodeIdentifier(field).IdentifierName),
                                _names.MakeTypeSyntax(elementType, field.DeclaringType, TypeUsage.DomainClass),
                                rank);

                case TypeTag.ListPointer:
                    return MakeAnyListConversion(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            _names.ReaderParameter.IdentifierName,
                            _names.GetCodeIdentifier(field).IdentifierName));
            }

            return MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                _names.ReaderParameter.IdentifierName,
                _names.GetCodeIdentifier(field).IdentifierName);
        }

        IEnumerable<SwitchSectionSyntax> MakeSerializeMethodSwitchSections(TypeDefinition def)
        {
            var unionFields = def.Fields.Where(f => f.DiscValue.HasValue);

            foreach (var unionField in unionFields)
            {
                var section = SwitchSection()
                    .AddLabels(
                        CaseSwitchLabel(MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            _names.UnionDiscriminatorEnum.IdentifierName,
                            _names.GetCodeIdentifier(unionField).IdentifierName)));

                if (unionField.Type.Tag != TypeTag.Void)
                {
                    ExpressionSyntax right = _names.GetCodeIdentifier(unionField).IdentifierName;

                    var syntax = _names.MakeTypeSyntax(unionField.Type, unionField.DeclaringType, TypeUsage.DomainClassNullable);

                    if (syntax is NullableTypeSyntax)
                    {
                        right = MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            right,
                            IdentifierName(nameof(Nullable<int>.Value)));
                    }

                    section = section.AddStatements(MakeSerializeMethodFieldAssignment(unionField));
                }

                section = section.AddStatements(BreakStatement());

                yield return section;
            }
        }

        IEnumerable<SwitchSectionSyntax> MakeDeserializeMethodSwitchSections(TypeDefinition def)
        {
            var unionFields = def.Fields.Where(f => f.DiscValue.HasValue);

            foreach (var unionField in unionFields)
            {
                var section = SwitchSection()
                    .AddLabels(
                        CaseSwitchLabel(MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            _names.UnionDiscriminatorEnum.IdentifierName,
                            _names.GetCodeIdentifier(unionField).IdentifierName)));

                switch (unionField.Type.Tag)
                {
                    case TypeTag.Void:
                        section = section.AddStatements(
                            ExpressionStatement(AssignmentExpression(
                                SyntaxKind.SimpleAssignmentExpression,
                                    _names.UnionDiscriminatorProp.IdentifierName,
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        _names.ReaderParameter.IdentifierName,
                                        _names.UnionDiscriminatorProp.IdentifierName))));
                        break;

                    default:
                        section = section.AddStatements(
                            ExpressionStatement(AssignmentExpression(
                                SyntaxKind.SimpleAssignmentExpression,
                                _names.GetCodeIdentifier(unionField).IdentifierName,
                                MakeDeserializeMethodRightHandSide(unionField))));
                        break;

                }

                section = section.AddStatements(BreakStatement());

                yield return section;
            }
        }

        IEnumerable<StatementSyntax> MakeSerializeStatements(TypeDefinition def)
        {
            if (def.UnionInfo != null)
            {
                yield return ExpressionStatement(AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        _names.WriterParameter.IdentifierName,
                        _names.UnionDiscriminatorProp.IdentifierName),
                    _names.UnionDiscriminatorProp.IdentifierName));

                yield return SwitchStatement(_names.UnionDiscriminatorProp.IdentifierName)
                    .WithOpenParenToken(Token(SyntaxKind.OpenParenToken))
                    .WithCloseParenToken(Token(SyntaxKind.CloseParenToken))
                    .AddSections(MakeSerializeMethodSwitchSections(def).ToArray());
            }

            var nondiscFields = def.Fields.Where(f => !f.DiscValue.HasValue && f.Type.Tag != TypeTag.Void);

            foreach (var field in nondiscFields)
            {
                var asmt = MakeSerializeMethodFieldAssignment(field);

                if (asmt != null)
                {
                    yield return asmt;
                }
            }
        }

        IEnumerable<StatementSyntax> MakeApplyDefaultsStatements(TypeDefinition def)
        {
            var relevantFields = def.Fields.Where(
                f => !f.DiscValue.HasValue && 
                f.Type.Tag != TypeTag.Void &&
                f.DefaultValueIsExplicit &&
                !f.Type.IsValueType);

            foreach (var field in relevantFields)
            {
                var asmt = MakeApplyDefaultsMethodFieldAssignment(field);

                if (asmt != null)
                {
                    yield return asmt;
                }
            }
        }

        MemberDeclarationSyntax MakeSerializeMethod(TypeDefinition def)
        {
            return MethodDeclaration(PredefinedType(
                Token(SyntaxKind.VoidKeyword)),
                _names.SerializeMethod.Identifier)
                .AddModifiers(Public)
                .AddParameterListParameters(
                    Parameter(_names.WriterParameter.Identifier)
                        .WithType(_names.WriterStruct.IdentifierName))
                .AddBodyStatements(MakeSerializeStatements(def).ToArray());
        }

        MemberDeclarationSyntax MakeSerializeInterfaceMethod()
        {
            return MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)),
                Identifier(nameof(Capnp.ICapnpSerializable.Serialize)))
                .WithExplicitInterfaceSpecifier(
                    ExplicitInterfaceSpecifier(IdentifierName(nameof(Capnp.ICapnpSerializable))))
                .AddParameterListParameters(
                    Parameter(_names.AnonymousParameter.Identifier)
                        .WithType(Type<Capnp.SerializerState>()))
                .AddBodyStatements(
                    ExpressionStatement(
                        InvocationExpression(_names.SerializeMethod.IdentifierName)
                        .AddArgumentListArguments(
                            Argument(
                                InvocationExpression(
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        _names.AnonymousParameter.IdentifierName,
                                        GenericName(Identifier(nameof(Capnp.SerializerState.Rewrap)))
                                        .AddTypeArgumentListArguments(_names.WriterStruct.IdentifierName)))))));
        }

        MemberDeclarationSyntax MakeApplyDefaultsMethod(TypeDefinition def)
        {
            return MethodDeclaration(PredefinedType(
                Token(SyntaxKind.VoidKeyword)),
                _names.ApplyDefaultsMethod.Identifier)
                .AddModifiers(Public)
                .AddBodyStatements(MakeApplyDefaultsStatements(def).ToArray());
        }

        IEnumerable<StatementSyntax> MakeDeserializeStatements(TypeDefinition def)
        {
            var relevantFields = def.Fields.Where(
                f => !f.DiscValue.HasValue && 
                f.Type.Tag != TypeTag.Void);

            foreach (var field in relevantFields)
            {
                var rhs = MakeDeserializeMethodRightHandSide(field);

                if (rhs != null)
                {
                    yield return ExpressionStatement(AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        _names.GetCodeIdentifier(field).IdentifierName,
                        rhs));
                }
            }
        }

        MemberDeclarationSyntax MakeDeserializeMethod(TypeDefinition def)
        {
            var stmts = new List<StatementSyntax>();

            stmts.Add(LocalDeclarationStatement(
                VariableDeclaration(IdentifierName("var"))
                .AddVariables(
                    VariableDeclarator(_names.ReaderParameter.Identifier)
                    .WithInitializer(
                        EqualsValueClause(
                            InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    _names.ReaderStruct.IdentifierName,
                                    _names.ReaderCreateMethod.IdentifierName))
                                    .AddArgumentListArguments(
                                        Argument(_names.AnonymousParameter.IdentifierName)))))));


            if (def.UnionInfo != null)
            {
                stmts.Add(SwitchStatement(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression, 
                            _names.ReaderParameter.IdentifierName,
                            _names.UnionDiscriminatorProp.IdentifierName))
                    .WithOpenParenToken(Token(SyntaxKind.OpenParenToken))
                    .WithCloseParenToken(Token(SyntaxKind.CloseParenToken))
                    .AddSections(MakeDeserializeMethodSwitchSections(def).ToArray()));
            }

            stmts.AddRange(MakeDeserializeStatements(def));
            stmts.Add(ExpressionStatement(InvocationExpression(
                    _names.ApplyDefaultsMethod.IdentifierName)));

            return MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)),
                Identifier(nameof(Capnp.ICapnpSerializable.Deserialize)))
                    .WithExplicitInterfaceSpecifier(
                        ExplicitInterfaceSpecifier(IdentifierName(nameof(Capnp.ICapnpSerializable))))
                    .AddParameterListParameters(
                        Parameter(_names.AnonymousParameter.Identifier)
                            .WithType(Type<Capnp.DeserializerState>()))
                .AddBodyStatements(stmts.ToArray());
        }

        IEnumerable<MemberDeclarationSyntax> EnumerateDomainClassMembers(TypeDefinition def)
        {
            yield return MakeDeserializeMethod(def);

            if (def.UnionInfo != null)
            {
                yield return MakeUnionDiscriminatorField();
                yield return MakeUnionContentField();
                yield return MakeUnionDiscriminatorProperty(def);
            }

            yield return MakeSerializeMethod(def);
            yield return MakeSerializeInterfaceMethod();
            yield return MakeApplyDefaultsMethod(def);

            foreach (var field in def.Fields)
            {
                var decl = MakeField(field);

                if (decl != null)
                    yield return decl;
            }
        }

        public MemberDeclarationSyntax[] MakeDomainClassMembers(TypeDefinition def)
        {
            return EnumerateDomainClassMembers(def).ToArray();
        }
    }
}
