using CapnpC.CSharp.Generator;
using System;
using System.Collections.Generic;
using System.IO;

namespace CapnpC.CSharp.MsBuild.Generation
{
    public class CapnpCodeBehindGenerator : IDisposable
    {

        public void InitializeProject(string projectPath)
        {
        }


        public CsFileGeneratorResult GenerateCodeBehindFile(string capnpFile)
        {
            // Works around a weird capnp.exe behavior: When the input file is empty, it will spit out an exception dump
            // instead of a parse error. But the parse error is nice because it contains a generated ID. We want the parse error!
            // Workaround: Generate a temporary file that contains a single line break (such that it is not empty...)
            try
            {
                if (File.Exists(capnpFile) && new FileInfo(capnpFile).Length == 0)
                {
                    string tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".capnp");

                    File.WriteAllText(tempFile, Environment.NewLine);
                    try
                    {
                        return GenerateCodeBehindFile(tempFile);
                    }
                    finally
                    {
                        File.Delete(tempFile);
                    }
                }
            }
            catch
            {
            }

            var result = CapnpCompilation.InvokeCapnpAndGenerate(new string[] { capnpFile });

            if (result.IsSuccess)
            {
                if (result.GeneratedFiles.Count == 1)
                {
                    return new CsFileGeneratorResult(
                        result.GeneratedFiles[0],
                        capnpFile + ".cs",
                        result.Messages);
                }
                else
                {
                    return new CsFileGeneratorResult(
                        "Code generation produced more than one file. This is not supported.",
                        result.Messages);
                }
            }
            else
            {
                switch (result.ErrorCategory)
                {
                    case CapnpProcessFailure.NotFound:
                        return new CsFileGeneratorResult("Unable to find capnp.exe - please install capnproto on your system first.");

                    case CapnpProcessFailure.BadInput:
                        return new CsFileGeneratorResult("Invalid schema", result.Messages);

                    case CapnpProcessFailure.BadOutput:
                        return new CsFileGeneratorResult(
                            "Internal error: capnp.exe produced a binary code generation request which was not understood by the backend",
                            result.Messages);

                    default:
                        throw new NotSupportedException("Invalid error category");
                }
            }
        }

        public void Dispose()
        {
        }
    }
}