
namespace FolderCompare
{
    using System.Collections.Generic;

    public class CompareContext
    {
        // Set by command line parameters
        public IMetadataSource LeftSource { get; set; }
        public IMetadataSource RightSource { get; set; }
        public DisplayMode OutputType { get; set; }

        // Set at runtime
        public IComparer<FileMetadata> Comparer { get; set; }
        public IEqualityComparer<FileMetadata> EqualityComparer { get; set; }
        public IComparisonReport Report { get; set; }

        public IEnumerable<FileMetadata> LeftItems { get; set; }
        public IEnumerable<FileMetadata> RightItems { get; set; }
    }
}
