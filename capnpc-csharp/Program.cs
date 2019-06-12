using Capnp;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace CapnpC
{
    class Program
    {
        static void Main(string[] args)
        {
            Stream input;

            if (args.Length > 0)
            {
                input = new FileStream(args[0], FileMode.Open, FileAccess.Read);
            }
            else
            {
                input = Console.OpenStandardInput();
            }

            WireFrame segments;

            using (input)
            {
                segments = Framing.ReadSegments(input);
            }

            var dec = DeserializerState.CreateRoot(segments);
            var reader = Schema.CodeGeneratorRequest.Reader.Create(dec);
            var model = Model.SchemaModel.Create(reader);
            var codeGen = new Generator.CodeGenerator(model, new Generator.GeneratorOptions());
            codeGen.Generate();
        }
    }
}
