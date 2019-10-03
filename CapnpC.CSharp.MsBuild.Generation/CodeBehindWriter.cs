using System;
using System.IO;
using Microsoft.Build.Utilities;

namespace CapnpC.CSharp.MsBuild.Generation
{
    public class CodeBehindWriter
    {
        public CodeBehindWriter(TaskLoggingHelper log)
        {
            Log = log;
        }

        public TaskLoggingHelper Log { get; }

        public string WriteCodeBehindFile(string outputPath, CsFileGeneratorResult testFileGeneratorResult)
        {
            string directoryPath = Path.GetDirectoryName(outputPath) ?? throw new InvalidOperationException();
            Log?.LogWithNameTag(Log.LogMessage, directoryPath);

            Log?.LogWithNameTag(Log.LogMessage, $"Writing data to {outputPath}; path = {directoryPath}; generatedFilename = {testFileGeneratorResult.Filename}");

            if (File.Exists(outputPath))
            {
                if (!FileSystemHelper.FileCompareContent(outputPath, testFileGeneratorResult.GeneratedCode))
                {
                    File.WriteAllText(outputPath, testFileGeneratorResult.GeneratedCode);
                }
            }
            else
            {
                File.WriteAllText(outputPath, testFileGeneratorResult.GeneratedCode);
            }

            return outputPath;
        }
    }
}
