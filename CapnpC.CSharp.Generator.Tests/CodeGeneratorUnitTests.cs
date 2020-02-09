using Capnp;
using Model = CapnpC.CSharp.Generator.Model;
using CodeGen = CapnpC.CSharp.Generator.CodeGen;
using CodeGeneratorRequest = CapnpC.CSharp.Generator.Schema.CodeGeneratorRequest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CapnpC.CSharp.Generator.Tests
{
    [TestClass]
    public class CodeGeneratorUnitTests
    {
        [TestMethod]
        public void Test00Enumerant()
        {
            var model = Load("UnitTest1.capnp.bin");
            Assert.AreEqual("@byte", GetTypeDef(0xc8461867c409f5d4, model).Enumerants[0].Literal);
        }

        [TestMethod]
        public void Test01NestedClash()
        {
            var (model, codegen, _) = LoadAndGenerate("UnitTest1.capnp.bin");
            var structFoo = GetTypeDef(0x93db6ba5509bac24, model);
            var names = codegen.GetNames();
            var fieldName = names.GetCodeIdentifier(structFoo.Fields[0]).ToString();
            Assert.AreEqual("Foo", structFoo.Name);
            Assert.AreNotEqual(structFoo.Name, fieldName);
        }

        [TestMethod]
        public void Test02ForwardInheritance()
        {
            LoadAndGenerate("UnitTest2.capnp.bin");
        }

        [TestMethod]
        public void Test03NonGeneratedNodeSkip()
        {
            LoadAndGenerate("UnitTest3.capnp.bin");
        }

        [TestMethod]
        public void Test04MutualDependencies()
        {
            LoadAndGenerate("UnitTest4.capnp.bin");
        }

        [TestMethod]
        public void Test10ImportedNamespaces()
        {
            var (model, codegen, _) = LoadAndGenerate("UnitTest10.capnp.bin");
            var outerTypeDef = GetGeneratedFile("UnitTest10.capnp", model).NestedTypes.First();
            var outerType = Model.Types.FromDefinition(outerTypeDef);
            var innerType = outerTypeDef.Fields[0].Type;
            var innerTypeDef = innerType.Definition;
            var names = codegen.GetNames();
            var outerNameSyntax = names.GetQName(outerType, outerTypeDef);
            var innerNameSyntax = names.GetQName(innerType, outerTypeDef);
            string[] outerNamespace = { "Foo", "Bar", "Baz" };
            string[] innerNamespace = { "Foo", "Garf", "Snarf" };
            CollectionAssert.AreEqual(outerNamespace, (outerTypeDef.DeclaringElement as Model.GenFile).Namespace);
            CollectionAssert.AreEqual(innerNamespace, (innerType.Definition.DeclaringElement as Model.GenFile).Namespace);
            string outerNSStr = String.Join('.', outerNamespace);
            string innerNSStr = String.Join('.', innerNamespace);
            Assert.AreEqual($"{outerNSStr}.Outer", outerNameSyntax.ToString());
            Assert.AreEqual($"{innerNSStr}.Inner", innerNameSyntax.ToString());
        }

        [TestMethod]
        public void Test11ImportedConst()
        {
            LoadAndGenerate("UnitTest11.capnp.bin");
        }

        [TestMethod]
        public void Test12ConstEnum()
        {
            LoadAndGenerate("UnitTest12.capnp.bin");
        }

        [TestMethod]
        public void Test13CSharpNamespace()
        {
            var (model, _, _) = LoadAndGenerate("UnitTest13.capnp.bin");
            var outerTypeDef = GetGeneratedFile("UnitTest13.capnp", model).NestedTypes.First();
            string[] outerNamespace = { "Foo", "Bar", "Baz" };
            CollectionAssert.AreEqual(outerNamespace, (outerTypeDef.DeclaringElement as Model.GenFile).Namespace);
        }

        [TestMethod]
        public void Test14CSharpNamespacePrecedesCxxNamespace()
        {
            var (model, _, _) = LoadAndGenerate("UnitTest14.capnp.bin");
            var outerTypeDef = GetGeneratedFile("UnitTest14.capnp", model).NestedTypes.First();
            string[] outerNamespace = { "Foo", "Bar", "Baz" };
            CollectionAssert.AreEqual(outerNamespace, (outerTypeDef.DeclaringElement as Model.GenFile).Namespace);
        }

        [TestMethod]
        public void Test15CSharpMemberNames()
        {
            var (_, _, code) = LoadAndGenerate("UnitTest15.capnp.bin");
            try
            {
                Assert.IsTrue(code.Contains("CsStruct", StringComparison.Ordinal), "Generated code must contain C# struct name");
                Assert.IsFalse(code.Contains("SomeStruct", StringComparison.Ordinal), "Generated code must not contain original struct name");
                Assert.IsTrue(code.Contains("CsField", StringComparison.Ordinal), "Generated code must contain C# field name");
                Assert.IsFalse(code.Contains("someField", StringComparison.OrdinalIgnoreCase), "Generated code must not contain original field name");
                Assert.IsTrue(code.Contains("CsUnion", StringComparison.Ordinal), "Generated code must contain C# union name");
                Assert.IsFalse(code.Contains("someUnion", StringComparison.OrdinalIgnoreCase), "Generated code must not contain original union name");
                Assert.IsTrue(code.Contains("CsGroup", StringComparison.Ordinal), "Generated code must contain C# group name");
                Assert.IsFalse(code.Contains("someGroup", StringComparison.OrdinalIgnoreCase), "Generated code must not contain original group name");
                Assert.IsTrue(code.Contains("CsEnum", StringComparison.Ordinal), "Generated code must contain C# enum name");
                Assert.IsFalse(code.Contains("SomeEnum", StringComparison.Ordinal), "Generated code must not contain original enum name");
                Assert.IsTrue(code.Contains("CsEnumerant", StringComparison.Ordinal), "Generated code must contain C# enumerant name");
                Assert.IsFalse(code.Contains("someEnumerant", StringComparison.OrdinalIgnoreCase), "Generated code must not contain original enumerant name");
                Assert.IsTrue(code.Contains("CsField", StringComparison.Ordinal), "Generated code must contain C# field name");
                Assert.IsFalse(code.Contains("someField", StringComparison.OrdinalIgnoreCase), "Generated code must not contain original field name");
                Assert.IsTrue(code.Contains("CsInterface", StringComparison.Ordinal), "Generated code must contain C# interface name");
                Assert.IsFalse(code.Contains("SomeInterface", StringComparison.Ordinal), "Generated code must not contain original interface name");
                Assert.IsTrue(code.Contains("CsMethod", StringComparison.Ordinal), "Generated code must contain C# method name");
                Assert.IsFalse(code.Contains("someMethod", StringComparison.OrdinalIgnoreCase), "Generated code must not contain original method name");
                Assert.IsTrue(code.Contains("CsField", StringComparison.Ordinal), "Generated code must contain C# field name");
                Assert.IsFalse(code.Contains("someField", StringComparison.OrdinalIgnoreCase), "Generated code must not contain original field name");
                Assert.IsTrue(code.Contains("CsResult", StringComparison.Ordinal), "Generated code must contain C# method parameter name");
                Assert.IsFalse(code.Contains("someResult", StringComparison.OrdinalIgnoreCase), "Generated code must not contain original method parameter name");
            }
            catch (AssertFailedException)
            {
                Console.WriteLine(code);
                throw;
            }
        }

        [TestMethod]
        public void Test20AnnotationAndConst()
        {
            LoadAndGenerate("UnitTest20.capnp.bin");
        }

        [TestMethod]
        public void Test30SchemaCapnp()
        {
            LoadAndGenerate("schema-with-offsets.capnp.bin");
        }

        static (Model.SchemaModel, CodeGen.CodeGenerator, string) LoadAndGenerate(string inputName)
        {
            var model = Load(inputName);
            var codegen = new CodeGen.CodeGenerator(model, new CodeGen.GeneratorOptions());

            var code = model.FilesToGenerate.Select(f => codegen.Transform(f)).ToArray();
            Assert.AreEqual(
                Util.InlineAssemblyCompiler.CompileSummary.Success,
                Util.InlineAssemblyCompiler.TryCompileCapnp(
                    Microsoft.CodeAnalysis.NullableContextOptions.Disable,
                    code), 
                "Compilation was not successful with no warnings");

            return (model, codegen, code[0]);
        }

        static Model.GenFile GetGeneratedFile(string name, Model.SchemaModel model)
        {
            var file = model.FilesToGenerate.SingleOrDefault(f => f.Name.EndsWith(name));
            Assert.IsNotNull(file, $"Could not find '{name}' in generated files");
            return file;
        }

        static Model.TypeDefinition GetTypeDef(ulong id, Model.SchemaModel model)
        {
            foreach (var defs in model.FilesToGenerate.Select(f => f.NestedTypes))
            {
                if (GetTypeDef(id, defs) is Model.TypeDefinition def) return def;
            }
            return null;
        }

        static Model.TypeDefinition GetTypeDef(ulong id, IEnumerable<Model.TypeDefinition> defs)
        {
            foreach (var def in defs)
            {
                if (def.Id == id) return def;
                var sub = GetTypeDef(id, def.NestedTypes);
                if (sub != null) return sub;
            }
            return null;
        }

        static Model.SchemaModel Load(string inputName)
        {
            WireFrame segments;
            var input = CodeGeneratorSteps.LoadResource(inputName);
            using (input)
            {
                segments = Framing.ReadSegments(input);
            }
            var dec = DeserializerState.CreateRoot(segments);
            var reader = CodeGeneratorRequest.Reader.Create(dec);
            var model = Model.SchemaModel.Create(reader);
            return model;
        }
    }
}
