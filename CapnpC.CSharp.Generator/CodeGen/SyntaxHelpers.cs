using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CapnpC.CSharp.Generator.CodeGen
{
    static class SyntaxHelpers
    {
        public static string MakeCamel(string name) => $"{char.ToUpperInvariant(name[0])}{name.Substring(1)}";
        public static string MakeAllLower(string name) => $"@{name}";

        public static readonly SyntaxToken Async = Token(SyntaxKind.AsyncKeyword);
        public static readonly SyntaxToken Public = Token(SyntaxKind.PublicKeyword);
        public static readonly SyntaxToken Private = Token(SyntaxKind.PrivateKeyword);
        public static readonly SyntaxToken Readonly = Token(SyntaxKind.ReadOnlyKeyword);
        public static readonly SyntaxToken Static = Token(SyntaxKind.StaticKeyword);
        public static readonly SyntaxToken Override = Token(SyntaxKind.OverrideKeyword);
        public static readonly SyntaxToken Partial = Token(SyntaxKind.PartialKeyword);
        public static readonly SyntaxToken This = Token(SyntaxKind.ThisKeyword);

        public static TypeSyntax Type(Type type)
        {
            switch (0)
            {
                case 0 when type == typeof(bool):
                    return PredefinedType(Token(SyntaxKind.BoolKeyword));

                case 0 when type == typeof(sbyte):
                    return PredefinedType(Token(SyntaxKind.SByteKeyword));

                case 0 when type == typeof(byte):
                    return PredefinedType(Token(SyntaxKind.ByteKeyword));

                case 0 when type == typeof(short):
                    return PredefinedType(Token(SyntaxKind.ShortKeyword));

                case 0 when type == typeof(ushort):
                    return PredefinedType(Token(SyntaxKind.UShortKeyword));

                case 0 when type == typeof(int):
                    return PredefinedType(Token(SyntaxKind.IntKeyword));

                case 0 when type == typeof(uint):
                    return PredefinedType(Token(SyntaxKind.UIntKeyword));

                case 0 when type == typeof(long):
                    return PredefinedType(Token(SyntaxKind.LongKeyword));

                case 0 when type == typeof(ulong):
                    return PredefinedType(Token(SyntaxKind.ULongKeyword));

                case 0 when type == typeof(float):
                    return PredefinedType(Token(SyntaxKind.FloatKeyword));

                case 0 when type == typeof(double):
                    return PredefinedType(Token(SyntaxKind.DoubleKeyword));

                case 0 when type == typeof(string):
                    return PredefinedType(Token(SyntaxKind.StringKeyword));

                case 0 when type == typeof(object):
                    return PredefinedType(Token(SyntaxKind.ObjectKeyword));

                case 0 when type.IsGenericType:
                    return GenericName(type.Name.Substring(0, type.Name.IndexOf('`')))
                        .AddTypeArgumentListArguments(type.GetGenericArguments().Select(Type).ToArray());

                default:
                    return ParseTypeName(type.Name);
            }
        }

        public static TypeSyntax Type<T>() => Type(typeof(T));

        public static ExpressionSyntax ValueOf(object value)
        {
            switch (value)
            {
                case bool x:
                    return LiteralExpression(x ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression);

                case sbyte x:
                    return CastExpression(Type<sbyte>(), LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(x)));

                case byte x:
                    return CastExpression(Type<byte>(), LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(x)));

                case short x:
                    return CastExpression(Type<short>(), LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(x)));

                case ushort x:
                    return CastExpression(Type<ushort>(), LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(x)));

                case int x:
                    return LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(x));

                case uint x:
                    return LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(x));

                case long x:
                    return LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(x));

                case ulong x:
                    return LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(x));

                case float x:
                    return LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(x));

                case double x:
                    return LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(x));

                case string x:
                    return LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(x));

                case null:
                    return LiteralExpression(SyntaxKind.NullLiteralExpression);

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
