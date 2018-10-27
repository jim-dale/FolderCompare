
namespace FolderCompare
{
    using System.Collections.Generic;

    public class CompareContext
    {
        public IMetadataSource LeftSource { get; set; }
        public IMetadataSource RightSource { get; set; }
        public IComparer<FileMetadata> Comparer { get; set; }
        public IEqualityComparer<FileMetadata> EqualityComparer { get; set; }
        public DisplayType OutputType { get; set; }

        public IEnumerable<FileMetadata> LeftItems { get; set; }
        public IEnumerable<FileMetadata> RightItems { get; set; }
    }
}
