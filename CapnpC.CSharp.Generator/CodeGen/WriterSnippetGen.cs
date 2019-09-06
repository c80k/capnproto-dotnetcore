using System.Collections.Generic;
using System.Linq;
using CapnpC.CSharp.Generator.Model;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static CapnpC.CSharp.Generator.CodeGen.SyntaxHelpers;

namespace CapnpC.CSharp.Generator.CodeGen
{
    class WriterSnippetGen
    {
        readonly GenNames _names;

        public WriterSnippetGen(GenNames names)
        {
            _names = names;
        }

        IEnumerable<MemberDeclarationSyntax> MakeWriterStructMembers(TypeDefinition structType)
        {
            yield return ConstructorDeclaration(_names.WriterStruct.Identifier)
                .AddModifiers(Public)
                .WithBody(
                    Block(
                        ExpressionStatement(
                            InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    ThisExpression(),
                                    IdentifierName(SerializerStateWorder.SetStructName)))
                            .AddArgumentListArguments(
                                Argument(
                                    LiteralExpression(
                                        SyntaxKind.NumericLiteralExpression,
                                        Literal(structType.StructDataWordCount))),
                                Argument(
                                    LiteralExpression(
                                        SyntaxKind.NumericLiteralExpression,
                                        Literal(structType.StructPointerCount)))))));
        }

        IEnumerable<MemberDeclarationSyntax> MakeGroupWriterStructMembers()
        {
            yield return ConstructorDeclaration(_names.WriterStruct.Identifier)
                .AddModifiers(Public)
                .WithBody(Block());
        }

        PropertyDeclarationSyntax MakeWriterProperty(
            TypeSyntax type, 
            string name, 
            ExpressionSyntax getter, 
            ExpressionSyntax setter,
            bool cast, 
            bool cond)
        {
            if (cast)
            {
                getter = CastExpression(type, getter);
            }

            if (cond)
            {
                getter = ConditionalExpression(
                    BinaryExpression(
                        SyntaxKind.EqualsExpression,
                        _names.UnionDiscriminatorProp.IdentifierName,
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            _names.UnionDiscriminatorEnum.IdentifierName,
                            IdentifierName(name))),
                    getter,
                    LiteralExpression(
                        SyntaxKind.DefaultLiteralExpression,
                        Token(SyntaxKind.DefaultKeyword)));
            }

            var accessors = new AccessorDeclarationSyntax[setter != null ? 2 : 1];

            accessors[0] = AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithExpressionBody(ArrowExpressionClause(getter))
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

            if (setter != null)
            {
                accessors[1] = AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                            .WithExpressionBody(ArrowExpressionClause(setter))
                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
            }

            return PropertyDeclaration(type, name)
                .AddModifiers(Public)
                .AddAccessorListAccessors(accessors);
        }

        ExpressionSyntax MakePointerSyntax(TypeSyntax type, object index) =>
            InvocationExpression(
                GenericName(nameof(Capnp.SerializerState.BuildPointer))
                    .AddTypeArgumentListArguments(type))
                    .AddArgumentListArguments(
                        Argument(ValueOf(index)));

        ExpressionSyntax MakeReadCapSyntax(TypeSyntax type, object index) =>
            InvocationExpression(
                GenericName(nameof(Capnp.SerializerState.ReadCap))
                    .AddTypeArgumentListArguments(type))
                    .AddArgumentListArguments(
                        Argument(ValueOf(index)));

        ExpressionSyntax MakeTypedPointerSyntax(object index, TypeSyntax type) =>
            InvocationExpression(
                GenericName(nameof(Capnp.SerializerState.BuildPointer))
                    .AddTypeArgumentListArguments(type))
                    .AddArgumentListArguments(
                        Argument(ValueOf(index)));

        ExpressionSyntax MakeLinkSyntax(object index) =>
            InvocationExpression(
                IdentifierName(SerializerStateWorder.LinkName))
                .AddArgumentListArguments(
                    Argument(ValueOf(index)),
                    Argument(IdentifierName("value")));

        ExpressionSyntax MakeLinkObjectSyntax(object index) =>
            InvocationExpression(
                IdentifierName(nameof(Capnp.SerializerState.LinkObject)))
                .AddArgumentListArguments(
                    Argument(ValueOf(index)),
                    Argument(IdentifierName("value")));

        PropertyDeclarationSyntax MakePointerProperty(TypeSyntax type, string name, object index, bool cast, bool cond)
        {
            ExpressionSyntax getter = MakePointerSyntax(type, index);
            ExpressionSyntax setter = MakeLinkSyntax(index);

            return MakeWriterProperty(type, name, getter, setter, cast, cond);
        }

        PropertyDeclarationSyntax MakePointerAsStructProperty(
            TypeSyntax type, string name, object index,
            bool cast, bool cond)
        {
            ExpressionSyntax getter = MakeTypedPointerSyntax(index, type);
            ExpressionSyntax setter = MakeLinkSyntax(index);

            return MakeWriterProperty(type, name, getter, setter, cast, cond);
        }

        PropertyDeclarationSyntax MakeProperty(
            TypeSyntax outerType, 
            TypeSyntax innerType,
            string name, 
            string readName, 
            string writeName,
            object indexOrBitOffset,
            ExpressionSyntax secondArg, 
            bool cast, 
            bool cond,
            bool pasd)
        {
            var getter = InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    ThisExpression(),
                    IdentifierName(readName)))
                    .AddArgumentListArguments(
                        Argument(ValueOf(indexOrBitOffset)),
                        Argument(secondArg));

            ExpressionSyntax value = IdentifierName("value");

            if (cast)
            {
                value = CastExpression(innerType, value);
            }

            var setter = InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    ThisExpression(),
                    IdentifierName(writeName)))
                    .AddArgumentListArguments(
                        Argument(ValueOf(indexOrBitOffset)),
                        Argument(value),
                        Argument(secondArg));

            if (pasd)
            {
                setter.AddArgumentListArguments(Argument(secondArg));
            }

            return MakeWriterProperty(outerType, name, getter, setter, cast, cond);
        }

        PropertyDeclarationSyntax MakePrimitiveProperty<T>(Field field, string readName)
        {
            return MakeProperty(Type<T>(), null, _names.GetCodeIdentifier(field).ToString(), 
                readName, 
                nameof(Capnp.SerializerExtensions.WriteData),
                field.BitOffset.Value,
                ValueOf(field.DefaultValue.ScalarValue), 
                false, 
                field.DiscValue.HasValue,
                true);
        }

        PropertyDeclarationSyntax MakeEnumProperty(Field field, string readName)
        {
            return MakeProperty(_names.MakeTypeSyntax(field.Type, field.DeclaringType, TypeUsage.NotRelevant), Type<ushort>(), 
                _names.GetCodeIdentifier(field).ToString(),
                readName,
                nameof(Capnp.SerializerExtensions.WriteData),
                field.BitOffset.Value,
                ValueOf(field.DefaultValue.ScalarValue),
                true,
                field.DiscValue.HasValue,
                true);
        }

        PropertyDeclarationSyntax MakeTextProperty(Field field)
        {
            return MakeProperty(Type<string>(), null, 
                _names.GetCodeIdentifier(field).ToString(),
                nameof(Capnp.SerializerState.ReadText),
                nameof(Capnp.SerializerState.WriteText),
                (int)field.Offset,
                ValueOf(field.DefaultValue.ScalarValue), 
                false, 
                field.DiscValue.HasValue,
                false);
        }

        PropertyDeclarationSyntax MakeStructProperty(Field field)
        {
            var qtype = _names.MakeTypeSyntax(field.Type, field.DeclaringType, TypeUsage.Writer);

            return MakePointerAsStructProperty(qtype, _names.GetCodeIdentifier(field).ToString(),
                (int)field.Offset, false, field.DiscValue.HasValue);
        }

        PropertyDeclarationSyntax MakeGroupProperty(Field field)
        {
            var type = QualifiedName(
                _names.MakeTypeName(field.Type.Definition).IdentifierName,
                _names.WriterStruct.IdentifierName);

            var getter = InvocationExpression(
                GenericName(nameof(Capnp.SerializerState.Rewrap))
                    .AddTypeArgumentListArguments(type));

            return MakeWriterProperty(type, _names.GetCodeIdentifier(field).ToString(), getter, null, false, field.DiscValue.HasValue);
        }

        PropertyDeclarationSyntax MakeListProperty(Field field)
        {
            var qtype = _names.MakeTypeSyntax(field.Type, field.DeclaringType, TypeUsage.Writer);

            return MakePointerProperty(qtype, _names.GetCodeIdentifier(field).ToString(),
                (int)field.Offset, false, field.DiscValue.HasValue);
        }

        PropertyDeclarationSyntax MakePointerProperty(Field field)
        {
            var type = IdentifierName(nameof(Capnp.DynamicSerializerState));

            return MakePointerProperty(type, _names.GetCodeIdentifier(field).ToString(), (int)field.Offset, false, field.DiscValue.HasValue);
        }

        PropertyDeclarationSyntax MakeCapProperty(Field field)
        {
            var type = _names.MakeTypeSyntax(field.Type, field.DeclaringType, TypeUsage.Writer);
            int index = (int)field.Offset;
            string name = _names.GetCodeIdentifier(field).ToString();
            ExpressionSyntax getter = MakeReadCapSyntax(type, index);
            ExpressionSyntax setter = MakeLinkObjectSyntax(index);

            return MakeWriterProperty(type, name, getter, setter, false, field.DiscValue.HasValue);
        }

        PropertyDeclarationSyntax MakeWriterUnionSelector(TypeDefinition def)
        {
            return MakeProperty(
                _names.UnionDiscriminatorEnum.IdentifierName,
                Type<ushort>(),
                _names.UnionDiscriminatorProp.ToString(),
                nameof(Capnp.SerializerExtensions.ReadDataUShort),
                nameof(Capnp.SerializerExtensions.WriteData),
                def.UnionInfo.TagOffset,
                ValueOf(default(ushort)),
                true, false, true);
        }

        PropertyDeclarationSyntax MakeWriterFieldProperty(Field field)
        {
            switch (field.Type.Tag)
            {
                case TypeTag.Bool:
                    return MakePrimitiveProperty<bool>(field,
                        nameof(Capnp.SerializerExtensions.ReadDataBool));

                case TypeTag.S8:
                    return MakePrimitiveProperty<sbyte>(field,
                        nameof(Capnp.SerializerExtensions.ReadDataSByte));

                case TypeTag.U8:
                    return MakePrimitiveProperty<byte>(field,
                        nameof(Capnp.SerializerExtensions.ReadDataByte));

                case TypeTag.S16:
                    return MakePrimitiveProperty<short>(field,
                        nameof(Capnp.SerializerExtensions.ReadDataShort));

                case TypeTag.U16:
                    return MakePrimitiveProperty<ushort>(field,
                        nameof(Capnp.SerializerExtensions.ReadDataUShort));

                case TypeTag.S32:
                    return MakePrimitiveProperty<int>(field,
                        nameof(Capnp.SerializerExtensions.ReadDataInt));

                case TypeTag.U32:
                    return MakePrimitiveProperty<uint>(field,
                        nameof(Capnp.SerializerExtensions.ReadDataUInt));

                case TypeTag.S64:
                    return MakePrimitiveProperty<long>(field,
                        nameof(Capnp.SerializerExtensions.ReadDataLong));

                case TypeTag.U64:
                    return MakePrimitiveProperty<ulong>(field,
                        nameof(Capnp.SerializerExtensions.ReadDataULong));

                case TypeTag.F32:
                    return MakePrimitiveProperty<float>(field,
                        nameof(Capnp.SerializerExtensions.ReadDataFloat));

                case TypeTag.F64:
                    return MakePrimitiveProperty<double>(field,
                        nameof(Capnp.SerializerExtensions.ReadDataDouble));

                case TypeTag.Enum:
                    return MakeEnumProperty(field, nameof(Capnp.SerializerExtensions.ReadDataUShort));

                case TypeTag.Text:
                    return MakeTextProperty(field);

                case TypeTag.Struct:
                    return MakeStructProperty(field);

                case TypeTag.Group:
                    return MakeGroupProperty(field);

                case TypeTag.List:
                case TypeTag.Data:
                    return MakeListProperty(field);

                case TypeTag.AnyPointer:
                case TypeTag.StructPointer:
                case TypeTag.ListPointer:
                    return MakePointerProperty(field);

                case TypeTag.CapabilityPointer:
                case TypeTag.Interface:
                    return MakeCapProperty(field);

                default:
                    return null;
            }
        }

        public ClassDeclarationSyntax MakeWriterStruct(TypeDefinition def)
        {
            var WriterDecl = ClassDeclaration(_names.WriterStruct.ToString())
                .AddModifiers(Public)
                .AddBaseListTypes(
                    SimpleBaseType(IdentifierName(nameof(Capnp.SerializerState))));

            var members = def.Tag == TypeTag.Group ?
                MakeGroupWriterStructMembers() :
                MakeWriterStructMembers(def);

            WriterDecl = WriterDecl.AddMembers(members.ToArray());

            if (def.UnionInfo != null)
            {
                WriterDecl = WriterDecl.AddMembers(MakeWriterUnionSelector(def));
            }

            foreach (var field in def.Fields)
            {
                var propDecl = MakeWriterFieldProperty(field);

                if (propDecl != null)
                {
                    WriterDecl = WriterDecl.AddMembers(propDecl);
                }
            }

            return WriterDecl;
        }
    }
}
