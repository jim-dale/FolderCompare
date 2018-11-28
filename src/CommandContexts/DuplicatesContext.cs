
namespace FolderCompare
{
    using System.Collections.Generic;

    public class DuplicatesContext
    {
        public IMetadataSource Source { get; set; }

        public IComparisonReport Report { get; set; }

        public IEnumerable<FileMetadata> Items { get; set; }
    }
}
