using System.Collections.Generic;

namespace Capnpc.Csharp.MsBuild.Generation
{
    public interface ICapnpCsharpGenerator
    {
        IEnumerable<string> GenerateFilesForProject(string projectPath, string rootNamespace, List<string> CapnpFiles, List<string> generatorPlugins, string projectFolder, string outputPath);
    }
}