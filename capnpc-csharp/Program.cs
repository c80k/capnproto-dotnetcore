using Capnp;
using System;
using System.IO;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleToAttribute("capnpc-csharp.tests")]

namespace CapnpC
{
    internal class Program
    {
        internal static void GenerateFromStream(Stream input)
        {
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

        static void Main(string[] args)
        {
            Stream input;

            if (args.Length > 0)
            {
                input = new FileStream(args[0], FileMode.Open, FileAccess.Read);
            }
            else
            { 
                Console.WriteLine("Cap'n Proto C# code generator backend");
                Console.WriteLine("expecting binary-encoded code generation request from standard input");

                input = Console.OpenStandardInput();
            }

            try
            {
                GenerateFromStream(input);
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception.Message);
                Environment.ExitCode = -1;
            }
        }
    }
}
