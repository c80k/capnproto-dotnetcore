using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Utilities;

namespace Capnpc.Csharp.MsBuild.Generation
{
    public class CapnpFileCodeBehindGenerator : ICapnpcCsharpGenerator
    {
        private readonly FilePathGenerator _filePathGenerator;
        
        public CapnpFileCodeBehindGenerator(TaskLoggingHelper log)
        {
            Log = log ?? throw new ArgumentNullException(nameof(log));
            _filePathGenerator = new FilePathGenerator();
        }

        public TaskLoggingHelper Log { get; }

        public IEnumerable<string> GenerateFilesForProject(
            string projectPath,
            List<string> capnpFiles,
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

                foreach (var capnpFile in capnpFiles)
                {
                    var capnpFileItemSpec = capnpFile;
                    var generatorResult = capnpCodeBehindGenerator.GenerateCodeBehindFile(capnpFileItemSpec);

                    if (!generatorResult.Success)
                    {
                        foreach (var error in generatorResult.Errors)
                        {
                            //Log.LogError(
                            //    null,
                            //    null,
                            //    null,
                            //    featureFile,
                            //    error.Line,
                            //    error.LinePosition,
                            //    0,
                            //    0,
                            //    error.Message);
                        }
                        continue;
                    }

                    var targetFilePath = _filePathGenerator.GenerateFilePath(
                        projectFolder,
                        capnpFile,
                        generatorResult.Filename);

                    var resultedFile = codeBehindWriter.WriteCodeBehindFile(targetFilePath, capnpFile, generatorResult);

                    yield return FileSystemHelper.GetRelativePath(resultedFile, projectFolder);
                }
            }

        }
    }
}
