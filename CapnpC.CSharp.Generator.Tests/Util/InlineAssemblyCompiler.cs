using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace CapnpC.CSharp.Generator.Tests.Util
{
    class InlineAssemblyCompiler
    {
        public static bool TryCompileCapnp(string code)
        {
            return TryCompileCapnp(new[] {code});
        }

        public static bool TryCompileCapnp(string[] code)
        {
            var options = new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary, 
                optimizationLevel: OptimizationLevel.Debug);

            string assemblyRoot = Path.GetDirectoryName(typeof(object).Assembly.Location);

            string capnpRuntimePath = Path.GetFullPath(Path.Combine(
                Assembly.GetExecutingAssembly().Location,
                @"..\..\..\..\..\Capnp.Net.Runtime\bin\Debug\netcoreapp2.1\Capnp.Net.Runtime.dll"));

            var compilation = CSharpCompilation.Create(
                "CompilationTestAssembly",
                options: options,
                references: new MetadataReference[] {
                    MetadataReference.CreateFromFile(Path.Combine(assemblyRoot, "mscorlib.dll")),
                    MetadataReference.CreateFromFile(Path.Combine(assemblyRoot, "System.dll")),
                    MetadataReference.CreateFromFile(Path.Combine(assemblyRoot, "System.Core.dll")),
                    MetadataReference.CreateFromFile(Path.Combine(assemblyRoot, "System.Runtime.dll")),
                    MetadataReference.CreateFromFile(Path.Combine(assemblyRoot, "System.Private.CoreLib.dll")),
                    MetadataReference.CreateFromFile(capnpRuntimePath) },
                syntaxTrees: Array.ConvertAll(code, new Converter<string, SyntaxTree>(c => CSharpSyntaxTree.ParseText(c))));

            using (var stream = new MemoryStream())
            {
                var emitResult = compilation.Emit(stream);

                foreach (var diag in emitResult.Diagnostics)
                    Console.WriteLine($"{diag}");

                if (!emitResult.Success)
                {
                    foreach (var c in code)
                    {
                        string path = Path.ChangeExtension(Path.GetTempFileName(), ".capnp.cs");
                        File.WriteAllText(path, c);
                        Console.WriteLine($"[See {path} for generated code]");
                    }
                }

                return emitResult.Success;
            }
        }
    }
}
