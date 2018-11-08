
namespace FolderCompare
{
    using System.Collections.Generic;

    public class MovedContext
    {
        // Set by command line parameters
        public IMetadataSource LeftSource { get; set; }
        public IMetadataSource RightSource { get; set; }
        public DisplayMode OutputType { get; set; }

        // Set at runtime
        public IEqualityComparer<FileMetadata> EqualityComparer { get; set; }
        public IEqualityComparer<FileMetadata> ContentsComparer { get; set; }

        public IComparisonReport Report { get; set; }

        public IEnumerable<FileMetadata> LeftItems { get; set; }
        public IEnumerable<FileMetadata> RightItems { get; set; }

    }
}
