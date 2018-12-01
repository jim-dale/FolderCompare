
namespace FolderCompare
{
    public partial class CompareContext
    {
        internal class Pair
        {
            public FileMetadata LeftItem { get; set; }
            public FileMetadata RightItem { get; set; }

            public static Pair Create(FileMetadata leftItem, FileMetadata rightItem)
            {
                return new Pair { LeftItem = leftItem, RightItem = rightItem };
            }
        }
    }
}
