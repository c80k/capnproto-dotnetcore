using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CapnpC.CSharp.Generator.Tests.Util
{
    class InlineAssemblyCompiler
    {
        public enum CompileSummary
        {
            Success,
            SuccessWithWarnings,
            Error
        }

        public static CompileSummary TryCompileCapnp(NullableContextOptions nullableContextOptions, params string[] code)
        {
            var options = new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary, 
                optimizationLevel: OptimizationLevel.Debug,
                nullableContextOptions: nullableContextOptions);

            string assemblyRoot = Path.GetDirectoryName(typeof(object).Assembly.Location);

            string capnpRuntimePath = Path.GetFullPath(Path.Combine(
                Assembly.GetExecutingAssembly().Location,
                "..", "..", "..", "..", "..",
                "Capnp.Net.Runtime",
                "bin",
                "Debug",
                "net6.0",
                "Capnp.Net.Runtime.dll"));

            var parseOptions = CSharpParseOptions.Default;
            if (nullableContextOptions == NullableContextOptions.Disable)
                parseOptions = parseOptions.WithLanguageVersion(LanguageVersion.CSharp7_1);

            var compilation = CSharpCompilation.Create(
                "CompilationTestAssembly",
                options: options,
                references: new MetadataReference[] {
                    MetadataReference.CreateFromFile(Path.Combine(assemblyRoot, "mscorlib.dll")),
                    MetadataReference.CreateFromFile(Path.Combine(assemblyRoot, "System.dll")),
                    MetadataReference.CreateFromFile(Path.Combine(assemblyRoot, "System.Core.dll")),
                    MetadataReference.CreateFromFile(Path.Combine(assemblyRoot, "System.Diagnostics.Tools.dll")),
                    MetadataReference.CreateFromFile(Path.Combine(assemblyRoot, "System.Runtime.dll")),
                    MetadataReference.CreateFromFile(Path.Combine(assemblyRoot, "System.Private.CoreLib.dll")),
                    MetadataReference.CreateFromFile(capnpRuntimePath) },
                syntaxTrees: Array.ConvertAll(code, new Converter<string, SyntaxTree>(c => CSharpSyntaxTree.ParseText(c, parseOptions))));

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

                if (emitResult.Success)
                {
                    if (emitResult.Diagnostics.Any(diag => diag.Severity == DiagnosticSeverity.Warning))
                        return CompileSummary.SuccessWithWarnings;
                    else
                        return CompileSummary.Success;
                }
                else
                {
                    return CompileSummary.Error;
                }
            }
        }
    }
}
