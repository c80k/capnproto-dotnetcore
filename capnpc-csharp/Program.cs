using Capnp;
using CapnpC.CSharp.Generator;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security;

namespace CapnpC
{
    internal class Program
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

            var result = CapnpCompilation.GenerateFromStream(input);

            if (result.IsSuccess)
            {
                foreach (var generatedFile in result.GeneratedFiles)
                {
                    if (generatedFile.IsSuccess)
                    {
                        string outputFile = generatedFile.CapnpFilePath + ".cs";

                        try
                        {
                            File.WriteAllText(outputFile, generatedFile.GeneratedContent);
                        }
                        catch (Exception exception)
                        {
                            Console.Error.WriteLine(exception.Message);
                            Environment.ExitCode = -1;
                        }
                    }
                    else
                    {
                        Console.Error.WriteLine($"Error generating {generatedFile.CapnpFilePath}: {generatedFile.Exception.Message}");
                        Environment.ExitCode = -1;
                    }
                }
            }
            else
            {
                Console.Error.WriteLine(result.Exception.Message);
                Environment.ExitCode = -1;
            }
        }
    }
}
