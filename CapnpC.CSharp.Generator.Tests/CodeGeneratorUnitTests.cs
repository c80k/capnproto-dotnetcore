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
        static readonly Dictionary<int, string> GeneratedCode = new Dictionary<int, string>();

        [TestMethod]
        public void Test00Enumerant()
        {
            var model = Load("UnitTest1.capnp.bin");
            Assert.AreEqual("@byte", GetTypeDef(0xc8461867c409f5d4, model).Enumerants[0].Literal);
        }

        [TestMethod]
        public void Test01NestedClash()
        {
            var run = LoadAndGenerate("UnitTest1.capnp.bin", 1);
            var structFoo = GetTypeDef(0x93db6ba5509bac24, run.Model);
            var names = run.CodeGen.GetNames();
            var fieldName = names.GetCodeIdentifier(structFoo.Fields[0]).ToString();
            Assert.AreEqual("Foo", structFoo.Name);
            Assert.AreNotEqual(structFoo.Name, fieldName);
        }

        [TestMethod]
        public void Test02ForwardInheritance()
        {
            LoadAndGenerate("UnitTest2.capnp.bin", 2);
            // Should not throw
        }

        [TestMethod]
        public void Test03NonGeneratedNodeSkip()
        {
            LoadAndGenerate("UnitTest3.capnp.bin", 3);
            // Should not throw
        }

        [TestMethod]
        public void Test04MutualDependencies()
        {
            LoadAndGenerate("UnitTest4.capnp.bin", 4);
            // Should not throw
        }

        [TestMethod]
        public void Test10ImportedNamespaces()
        {
            var run = LoadAndGenerate("UnitTest10.capnp.bin", 10);
            var outerTypeDef = run.FirstFile.NestedTypes.First();
            var outerType = Model.Types.FromDefinition(outerTypeDef);
            var innerType = outerTypeDef.Fields[0].Type;
            var innerTypeDef = innerType.Definition;
            var names = run.CodeGen.GetNames();
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
            LoadAndGenerate("UnitTest11.capnp.bin", 11);
            // Should not throw
        }

        [TestMethod]
        public void Test20AnnotationAndConst()
        {
            LoadAndGenerate("UnitTest20.capnp.bin", 20);
            // Should not throw 
        }

        [TestMethod]
        public void Test30SchemaCapnp()
        {
            LoadAndGenerate("schema-with-offsets.capnp.bin");
            // Should not throw
        }

        struct Run
        {
            public Model.SchemaModel Model;
            public CodeGen.CodeGenerator CodeGen;
            public Model.GenFile FirstFile;
            public string Code;
        }

        static CodeGen.CodeGenerator NewGeneratorFor(Model.SchemaModel model)
            => new CodeGen.CodeGenerator(model, new CodeGen.GeneratorOptions());

        Run LoadAndGenerate(string inputName, int? testNum = null)
        {
            var run = new Run();
            run.Model = Load(inputName);
            run.CodeGen = NewGeneratorFor(run.Model);
            run.FirstFile = run.Model.FilesToGenerate.First();
            run.Code = run.CodeGen.Transform(run.FirstFile);
            if (testNum is int num)
                GeneratedCode[num] = run.Code;
            return run;
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
