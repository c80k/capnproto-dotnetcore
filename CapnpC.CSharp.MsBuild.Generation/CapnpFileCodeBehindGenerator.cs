using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace CapnpC.CSharp.MsBuild.Generation
{
    public class CapnpFileCodeBehindGenerator : ICapnpcCsharpGenerator
    {
        public CapnpFileCodeBehindGenerator(TaskLoggingHelper log)
        {
            Log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public TaskLoggingHelper Log { get; }

        public IEnumerable<string> GenerateFilesForProject(
            string projectPath,
            List<CapnpGenJob> capnpFiles,
            string projectFolder)
        {
            using (var capnpCodeBehindGenerator = new CapnpCodeBehindGenerator())
            {
                capnpCodeBehindGenerator.InitializeProject(projectPath);

                var codeBehindWriter = new CodeBehindWriter(null);

                if (capnpFiles == null)
                {
                    yield break;
                }

                foreach (var genJob in capnpFiles)
                {
                    Log.LogMessage(MessageImportance.Normal, "Generate {0}, working dir = {1}, options = {2}", 
                        genJob.CapnpPath,
                        genJob.WorkingDirectory,
                        string.Join(" ", genJob.AdditionalArguments));

                    var generatorResult = capnpCodeBehindGenerator.GenerateCodeBehindFile(genJob);

                    if (!generatorResult.Success)
                    {
                        if (!string.IsNullOrEmpty(generatorResult.Error))
                        {
                            Log.LogError("{0}", generatorResult.Error);
                        }

                        if (generatorResult.Messages != null)
                        {
                            foreach (var message in generatorResult.Messages)
                            {
                                if (message.IsParseSuccess)
                                {
                                    Log.LogError(
                                        subcategory: null,
                                        errorCode: null,
                                        helpKeyword: null,
                                        file: genJob.CapnpPath,
                                        lineNumber: message.Line,
                                        columnNumber: message.Column,
                                        endLineNumber: message.Line,
                                        endColumnNumber: message.EndColumn == 0 ? message.Column : message.EndColumn,
                                        "{0}",
                                        message.MessageText);
                                }
                                else
                                {
                                    Log.LogError("{0}", message.FullMessage);
                                }
                            }
                        }

                        continue;
                    }

                    var resultedFile = codeBehindWriter.WriteCodeBehindFile(generatorResult.Filename, generatorResult);

                    yield return FileSystemHelper.GetRelativePath(resultedFile, projectFolder);
                }
            }

        }
    }
}
