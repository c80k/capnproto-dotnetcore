using System.Collections.Generic;

namespace CapnpC.CSharp.MsBuild.Generation
{
    public class CapnpGenJob
    {
        public string CapnpPath { get; set; }
        public string WorkingDirectory { get; set; }
        public List<string> AdditionalArguments { get; } = new List<string>();
    }
}