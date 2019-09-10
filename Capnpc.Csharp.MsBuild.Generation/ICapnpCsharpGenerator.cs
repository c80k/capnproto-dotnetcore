using System.Collections.Generic;

namespace CapnpC.CSharp.MsBuild.Generation
{
    public interface ICapnpcCsharpGenerator
    {
        IEnumerable<string> GenerateFilesForProject(string projectPath, List<string> capnpFiles, string projectFolder, string workingDirectory, string additionalOptions);
    }
}