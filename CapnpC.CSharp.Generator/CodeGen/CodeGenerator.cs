namespace CapnpC.CSharp.Generator.CodeGen
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using CapnpC.CSharp.Generator.Model;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
    using static SyntaxHelpers;

    internal class CodeGenerator
    {
        readonly SchemaModel _model;
        readonly GenNames _names;
        readonly CommonSnippetGen _commonGen;
        readonly DomainClassSnippetGen _domClassGen;
        readonly ReaderSnippetGen _readerGen;
        readonly WriterSnippetGen _writerGen;
        readonly InterfaceSnippetGen _interfaceGen;

        public CodeGenerator(SchemaModel model, GeneratorOptions options)
        {
            _model = model;
            _names = new GenNames(options);
            _commonGen = new CommonSnippetGen(_names);
            _domClassGen = new DomainClassSnippetGen(_names);
            _readerGen = new ReaderSnippetGen(_names);
            _writerGen = new WriterSnippetGen(_names);
            _interfaceGen = new InterfaceSnippetGen(_names);
        }

        internal GenNames GetNames() => _names;

        IEnumerable<MemberDeclarationSyntax> TransformEnum(TypeDefinition def)
        {
            yield return _commonGen.MakeEnum(def);
        }

        IEnumerable<TypeParameterSyntax> MakeTypeParameters(TypeDefinition def)
        {
            foreach (string name in def.GenericParameters)
            {
                yield return TypeParameter(_names.GetGenericTypeParameter(name).Identifier);
            }
        }

        IEnumerable<TypeParameterConstraintClauseSyntax> MakeTypeParameterConstraints(TypeDefinition def)
        {
            foreach (string name in def.GenericParameters)
            {
                yield return TypeParameterConstraintClause(
                    _names.GetGenericTypeParameter(name).IdentifierName)
                        .AddConstraints(ClassOrStructConstraint(SyntaxKind.ClassConstraint));
            }
        }

        IEnumerable<MemberDeclarationSyntax> TransformStruct(TypeDefinition def)
        {
            var topDecl = ClassDeclaration(_names.MakeTypeName(def).Identifier)                
                .AddModifiers(Public)
                .AddBaseListTypes(SimpleBaseType(Type<Capnp.ICapnpSerializable>()));

            if (def.GenericParameters.Count > 0)
            {
                topDecl = topDecl
                    .AddTypeParameterListParameters(MakeTypeParameters(def).ToArray())
                    .AddConstraintClauses(MakeTypeParameterConstraints(def).ToArray());
            }

            topDecl = topDecl.AddMembers(CommonSnippetGen.MakeTypeIdConst(def.Id, _names));
            topDecl = topDecl.WithAttributeLists(CommonSnippetGen.MakeTypeIdAttributeLists(def.Id));

            if (def.UnionInfo != null)
            {
                topDecl = topDecl.AddMembers(_commonGen.MakeUnionSelectorEnum(def));
            }

            topDecl = topDecl.AddMembers(_domClassGen.MakeDomainClassMembers(def));
            topDecl = topDecl.AddMembers(_readerGen.MakeReaderStruct(def));
            topDecl = topDecl.AddMembers(_writerGen.MakeWriterStruct(def));

            foreach (var nestedGroup in def.NestedGroups)
            {
                topDecl = topDecl.AddMembers(Transform(nestedGroup).ToArray());
            }

            foreach (var nestedDef in def.NestedTypes)
            {
                topDecl = topDecl.AddMembers(Transform(nestedDef).ToArray());
            }

            yield return topDecl;
        }

        IEnumerable<MemberDeclarationSyntax> TransformInterface(TypeDefinition def)
        {
            yield return _interfaceGen.MakeInterface(def);
            yield return _interfaceGen.MakeProxy(def);
            yield return _interfaceGen.MakeSkeleton(def);

            if (_interfaceGen.RequiresPipeliningSupport(def))
            {
                yield return _interfaceGen.MakePipeliningSupport(def);
            }

            if (def.NestedTypes.Any())
            {
                var ns = ClassDeclaration(
                    _names.MakeTypeName(def, NameUsage.Namespace).ToString())
                    .AddModifiers(Public, Static);

                if (def.GenericParameters.Count > 0)
                {
                    ns = ns
                        .AddTypeParameterListParameters(MakeTypeParameters(def).ToArray())
                        .AddConstraintClauses(MakeTypeParameterConstraints(def).ToArray());
                }

                foreach (var nestedDef in def.NestedTypes)
                {
                    ns = ns.AddMembers(Transform(nestedDef).ToArray());
                }

                yield return ns;
            }
        }

        IEnumerable<MemberDeclarationSyntax> Transform(TypeDefinition def)
        {
            switch (def.Tag)
            {
                case TypeTag.Enum:
                    return TransformEnum(def);

                case TypeTag.Group:
                case TypeTag.Struct:
                    return TransformStruct(def);

                case TypeTag.Interface:
                    return TransformInterface(def);

                default:
                    throw new NotSupportedException($"Cannot declare type of kind {def.Tag} here");
            }
        }

        internal string Transform(GenFile file)
        {
            NameSyntax topNamespace = GenNames.NamespaceName(file.Namespace) ?? _names.TopNamespace;

            var ns = NamespaceDeclaration(topNamespace);

            foreach (var def in file.NestedTypes)
            {
                ns = ns.AddMembers(Transform(def).ToArray());
            }

            var cu = CompilationUnit().AddUsings(
                UsingDirective(ParseName("Capnp")),
                UsingDirective(ParseName("Capnp.Rpc")),
                UsingDirective(ParseName("System")),
                UsingDirective(ParseName("System.Collections.Generic")),
                UsingDirective(ParseName("System.Threading")),
                UsingDirective(ParseName("System.Threading.Tasks")));

            cu = cu.AddMembers(ns);

            return cu.NormalizeWhitespace().ToFullString();
        }

        public IReadOnlyList<FileGenerationResult> Generate()
        {
            var result = new List<FileGenerationResult>();

            foreach (var file in _model.FilesToGenerate)
            {
                try
                {
                    result.Add(new FileGenerationResult(file.Name, Transform(file)));
                }
                catch (System.Exception exception)
                {
                    result.Add(new FileGenerationResult(file.Name, exception));
                }
            }

            return result;
        }
    }
}
