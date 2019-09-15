using System.Collections.Generic;

namespace CapnpC.CSharp.MsBuild.Generation
{
    public interface ICapnpcCsharpGenerator
    {
        IEnumerable<string> GenerateFilesForProject(string projectPath, List<CapnpGenJob> jobs, string projectFolder);
    }
}