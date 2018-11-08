
namespace FolderCompare
{
    using System.Collections.Generic;

    public class CreateContext
    {
        public IMetadataSource Source { get; set; }
        public IMetadataTarget Target { get; set; }
        public bool GenerateContentsHash { get; set; }

        public IEnumerable<FileMetadata> Items { get; set; }
    }
}
