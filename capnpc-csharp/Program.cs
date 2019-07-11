using Capnp;
using System;
using System.IO;

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
                Console.WriteLine("Cap'n Proto C# code generator backend");
                Console.WriteLine("expecting binary-encoded code generation request from standard input");

                input = Console.OpenStandardInput();
            }

            try
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
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception.Message);
                Environment.ExitCode = -1;
            }
        }
    }
}
