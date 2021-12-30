using Capnp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("CapnpC.CSharp.Generator.Tests")]

namespace CapnpC.CSharp.Generator
{
    /// <summary>
    /// Provides methods for controlling both the C# code generator backend and the frontend "capnpc"
    /// </summary>
    public static class CapnpCompilation
    {
        /// <summary>
        /// Returns the basename of the capnp executable
        /// </summary>
        public static string CapnpCompilerFilename
        {
            get => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "capnp.exe" : "capnp";
        }

        /// <summary>
        /// Generates C# code from given input stream
        /// </summary>
        /// <param name="input">Input stream containing the binary code generation request, which the frontend capnpc emits</param>
        /// <param name="options">Configuration options for code generator. If null, default options will be used.</param>
        /// <returns>generation result</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="input"/> is null</exception>
        public static GenerationResult GenerateFromStream(Stream input, CodeGen.GeneratorOptions options)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            try
            {
                var segments = Framing.ReadSegments(input);
                var dec = DeserializerState.CreateRoot(segments);
                var reader = Schema.CodeGeneratorRequest.READER.create(dec);
                var model = Model.SchemaModel.Create(reader);
                var codeGen = new CodeGen.CodeGenerator(model, options ?? new CodeGen.GeneratorOptions());
                return new GenerationResult(codeGen.Generate());
            }
            catch (Exception exception)
            {
                return new GenerationResult(exception);
            }
        }

        /// <summary>
        /// Generates C# code from given input stream
        /// </summary>
        /// <param name="input">input stream containing the binary code generation request, which the frontend capnpc emits</param>
        /// <returns>generation result</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="input"/> is null</exception>
        public static GenerationResult GenerateFromStream(Stream input) => GenerateFromStream(input, null);

        /// <summary>
        /// Invokes "capnp.exe -o-" with given additional arguments and redirects the output to the C# generator backend.
        /// </summary>
        /// <param name="arguments">additional command line arguments</param>
        /// <param name="workingDirectory">optional working directory</param>
        /// <returns>generation result</returns>
        /// <exception cref="ArgumentNullException"><paramref name="arguments"/>is null</exception>
        public static GenerationResult InvokeCapnpAndGenerate(IEnumerable<string> arguments, string workingDirectory = null)
        {
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments));

            using (var compiler = new Process())
            {
                var argList = new List<string>();
                argList.Add("compile");
                argList.Add($"-o-");
                foreach (var arg in arguments)
                {
                    argList.Add($"\"{arg.TrimEnd('\\')}\"");
                }

                compiler.StartInfo.FileName = CapnpCompilerFilename;
                compiler.StartInfo.Arguments = string.Join(" ", argList);
                compiler.StartInfo.UseShellExecute = false;
                compiler.StartInfo.RedirectStandardOutput = true;
                compiler.StartInfo.RedirectStandardError = true;
                if (!string.IsNullOrWhiteSpace(workingDirectory))
                {
                    compiler.StartInfo.WorkingDirectory = workingDirectory;
                }

                try
                {
                    compiler.Start();
                }
                catch (Exception exception)
                {
                    return new GenerationResult(exception)
                    {
                        ErrorCategory = CapnpProcessFailure.NotFound
                    };
                }

                var result = GenerateFromStream(compiler.StandardOutput.BaseStream);

                var messageList = new List<CapnpMessage>();

                while (!compiler.StandardError.EndOfStream)
                {
                    messageList.Add(new CapnpMessage(compiler.StandardError.ReadLine()));
                }

                result.Messages = messageList;

                if (!result.IsSuccess)
                {
                    compiler.WaitForExit();
                    int exitCode = compiler.ExitCode;

                    if (exitCode == 0)
                        result.ErrorCategory = CapnpProcessFailure.BadOutput;
                    else
                        result.ErrorCategory = CapnpProcessFailure.BadInput;
                }

                return result;
            }
        }
    }
}
