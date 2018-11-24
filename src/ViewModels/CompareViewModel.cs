
using System;

namespace FolderCompare
{
    public class CompareViewModel
    {
        public FileMetadata LeftItem { get; set; }
        public FileMetadata RightItem { get; set; }
        public int RelPathComparison { get; set; }
        public int LastWriteComparison { get; set; }
        public int SizeComparison { get; set; }
        public int ContentsComparison{ get; set; }
    }
}
