using System;
using System.IO;

namespace Capnpc.Csharp.MsBuild.Generation
{
    public class FilePathGenerator
    {
        public string GenerateFilePath(string projectFolder, string capnpFileName, string generatedCodeBehindFileName)
        {
            if (projectFolder is null)
            {
                throw new ArgumentNullException(nameof(projectFolder));
            }

            if (capnpFileName is null)
            {
                throw new ArgumentNullException(nameof(capnpFileName));
            }

            if (generatedCodeBehindFileName is null)
            {
                throw new ArgumentNullException(nameof(generatedCodeBehindFileName));
            }

            string featureFileFullPath = Path.GetFullPath(Path.Combine(projectFolder, capnpFileName));
            string featureFileDirPath = Path.GetDirectoryName(featureFileFullPath);

            return Path.Combine(featureFileDirPath, generatedCodeBehindFileName);
        }
    }
}
