
namespace FolderCompare
{
    public class CompareViewModel
    {
        public FileMetadata LeftItem { get; set; }
        public FileMetadata RightItem { get; set; }
        public int Comparison { get; set; }
        public bool? AreEqual { get; set; }
    }
}
