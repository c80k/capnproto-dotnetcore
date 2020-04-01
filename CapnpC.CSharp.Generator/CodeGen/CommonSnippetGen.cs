using System.Collections.Generic;
using System.Linq;
using CapnpC.CSharp.Generator.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static CapnpC.CSharp.Generator.CodeGen.SyntaxHelpers;

namespace CapnpC.CSharp.Generator.CodeGen
{
    class CommonSnippetGen
    {
        readonly GenNames _names;

        public CommonSnippetGen(GenNames names)
        {
            _names = names;
        }

        public EnumDeclarationSyntax MakeUnionSelectorEnum(TypeDefinition def)
        {
            var whichEnum = EnumDeclaration(_names.UnionDiscriminatorEnum.ToString())
                .AddModifiers(Public)
                .AddBaseListTypes(SimpleBaseType(_names.Type<ushort>(Nullability.NonNullable)));

            var discFields = def.Fields.Where(f => f.DiscValue.HasValue);

            foreach (var discField in discFields)
            {
                whichEnum = whichEnum.AddMembers(
                    EnumMemberDeclaration(_names.GetCodeIdentifier(discField).Identifier)
                        .WithEqualsValue(
                            EqualsValueClause(LiteralExpression(
                                SyntaxKind.NumericLiteralExpression,
                                Literal(discField.DiscValue.Value)))));
            }

            var ndecl = EnumMemberDeclaration(_names.UnionDiscriminatorUndefined.ToString()).WithEqualsValue(
                EqualsValueClause(
                    LiteralExpression(
                        SyntaxKind.NumericLiteralExpression,
                        Literal(Schema.Field.Reader.NoDiscriminant))));

            whichEnum = whichEnum.AddMembers(ndecl);

            return whichEnum;
        }

        public EnumDeclarationSyntax MakeEnum(TypeDefinition def)
        {
            var decl = EnumDeclaration(_names.GetCodeIdentifier(def))
                .AddAttributeLists(_names.MakeTypeDecorationAttributes(def.Id))
                .AddModifiers(_names.TypeVisibilityModifier)
                .AddBaseListTypes(SimpleBaseType(_names.Type<ushort>(Nullability.NonNullable)));

            foreach (var enumerant in def.Enumerants.OrderBy(e => e.CodeOrder))
            {
                var mdecl = EnumMemberDeclaration(enumerant.CsLiteral ?? enumerant.Literal);

                if (enumerant.Ordinal.HasValue)
                {
                    mdecl = mdecl.WithEqualsValue(
                        EqualsValueClause(
                            LiteralExpression(
                                SyntaxKind.NumericLiteralExpression,
                                Literal(enumerant.Ordinal.Value))));
                }

                decl = decl.AddMembers(mdecl);
            }

            return decl;
        }

        public static IEnumerable<SyntaxNodeOrToken> MakeCommaSeparatedList(IEnumerable<ExpressionSyntax> expressions)
        {
            bool first = true;

            foreach (var expr in expressions)
            {
                if (first)
                    first = false;
                else
                    yield return Token(SyntaxKind.CommaToken);

                yield return expr;
            }
        }
    }
}
