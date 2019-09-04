using System.Collections.Generic;

namespace Capnpc.Csharp.MsBuild.Generation
{
    public class TestGeneratorResult
    {
        public IEnumerable<TestGenerationError> Errors { get; internal set; }
        public bool IsUpToDate { get; internal set; }
        public string GeneratedTestCode { get; internal set; }
    }
}