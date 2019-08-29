using capnpc_csharp.Tests.Properties;
using Capnp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CapnpC
{
    [TestClass]
    public class UnitTests
    {

        [TestMethod]
        public void Test00Enumerant()
        {
            var model = Load(Resources.UnitTest1_capnp);
            Assert.AreEqual("@byte", GetTypeDef(0xc8461867c409f5d4, model).Enumerants[0].Literal);
        }

        [TestMethod]
        public void Test01NestedClash()
        {
            var model = Load(Resources.UnitTest1_capnp);
            var structFoo = GetTypeDef(0x93db6ba5509bac24, model);
            var codeGen = NewGeneratorFor(model);
            codeGen.Transform(model.FilesToGenerate.First());
            var names = codeGen.GetNames();
            var fieldName = names.GetCodeIdentifier(structFoo.Fields[0]).ToString();
            Assert.AreEqual("Foo", structFoo.Name);
            Assert.AreNotEqual(structFoo.Name, fieldName);
        }

        [TestMethod]
        public void Test02ForwardInheritance()
        {
            var model = Load(Resources.UnitTest2_capnp);
            var codeGen = NewGeneratorFor(model);
            codeGen.Transform(model.FilesToGenerate.First());
            // Should not throw
        }

        [TestMethod]
        public void Test03NonGeneratedNodeSkip()
        {
            var model = Load(Resources.UnitTest3_capnp);
            // Should not throw
        }

        [TestMethod]
        public void Test10ImportedNamespaces()
        {
            var model = Load(Resources.UnitTest10_capnp);
            var codeGen = NewGeneratorFor(model);
            var genFile = model.FilesToGenerate.First();
            var outerTypeDef = genFile.NestedTypes.First();
            var outerType = Model.Types.FromDefinition(outerTypeDef);
            var innerType = outerTypeDef.Fields[0].Type;
            var innerTypeDef = innerType.Definition;
            var names = codeGen.GetNames();
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
        public void Test20AnnotationAndConst()
        {
            var model = Load(Resources.UnitTest20_capnp);
            var codeGen = NewGeneratorFor(model);
            codeGen.Transform(model.FilesToGenerate.First());
            // Should not throw 
        }

        [TestMethod]
        public void Test30SchemaCapnp()
        {
            var model = Load(Resources.schema_with_offsets_capnp);
            var codeGen = NewGeneratorFor(model);
            codeGen.Transform(model.FilesToGenerate.First());
            // Should not throw
        }

        static Generator.CodeGenerator NewGeneratorFor(Model.SchemaModel model)
            => new Generator.CodeGenerator(model, new Generator.GeneratorOptions());

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

        static Model.SchemaModel Load(byte[] data)
        {
            WireFrame segments;
            var input = new MemoryStream(data);
            using (input)
            {
                segments = Framing.ReadSegments(input);
            }
            var dec = DeserializerState.CreateRoot(segments);
            var reader = Schema.CodeGeneratorRequest.Reader.Create(dec);
            var model = Model.SchemaModel.Create(reader);
            return model;
        }
    }
}
