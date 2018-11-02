
namespace FolderCompare
{
    using System.Collections.Generic;

    public class HashComparer : Comparer<FileMetadata>
    {
        public override int Compare(FileMetadata x, FileMetadata y)
        {
            return string.Compare(x?.Hash, y?.Hash);
        }
    }
}
