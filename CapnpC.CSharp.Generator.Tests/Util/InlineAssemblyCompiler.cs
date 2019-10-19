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
            var options = new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary, 
                optimizationLevel: OptimizationLevel.Debug);

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);

            string assemblyRoot = Path.GetDirectoryName(typeof(object).Assembly.Location);

            string capnpRuntimePath = Path.GetFullPath(Path.Combine(
                Assembly.GetExecutingAssembly().Location,
                @"..\..\..\..\..\Capnp.Net.Runtime\bin\Debug\netcoreapp2.1\Capnp.Net.Runtime.dll"));

            var capnpRuntimeMetadataRef = MetadataReference.CreateFromFile(capnpRuntimePath);

            var compilation = CSharpCompilation.Create(
                "CompilationTestAssembly",
                options: options,
                references: new MetadataReference[] {
                    MetadataReference.CreateFromFile(Path.Combine(assemblyRoot, "mscorlib.dll")),
                    MetadataReference.CreateFromFile(Path.Combine(assemblyRoot, "System.dll")),
                    MetadataReference.CreateFromFile(Path.Combine(assemblyRoot, "System.Core.dll")),
                    MetadataReference.CreateFromFile(Path.Combine(assemblyRoot, "System.Runtime.dll")),
                    MetadataReference.CreateFromFile(Path.Combine(assemblyRoot, "System.Private.CoreLib.dll")),
                    capnpRuntimeMetadataRef }, 
                syntaxTrees: new SyntaxTree[] { syntaxTree });

            using (var stream = new MemoryStream())
            {
                var emitResult = compilation.Emit(stream);

                return emitResult.Success;
            }
        }
    }
}
