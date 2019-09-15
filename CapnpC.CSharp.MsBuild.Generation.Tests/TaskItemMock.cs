using System.Collections;
using Microsoft.Build.Framework;

namespace CapnpC.CSharp.MsBuild.Generation.Tests
{

    class TaskItemMock : ITaskItem
    {
        public string ItemSpec { get; set; }

        public ICollection MetadataNames => null;

        public int MetadataCount => 0;

        public IDictionary CloneCustomMetadata()
        {
            return null;
        }

        public void CopyMetadataTo(ITaskItem destinationItem)
        {
        }

        public string GetMetadata(string metadataName)
        {
            if (metadataName == "FullPath")
            {
                return ItemSpec;
            }

            return string.Empty;
        }

        public void RemoveMetadata(string metadataName)
        {
        }

        public void SetMetadata(string metadataName, string metadataValue)
        {
        }
    }
}
