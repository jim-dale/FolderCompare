
namespace FolderCompare
{
    using System.Collections.Generic;

    public class RelativePathHashComparer : Comparer<FileMetadata>
    {
        public override int Compare(FileMetadata x, FileMetadata y)
        {
            return string.Compare(x?.RelativePathHash, y?.RelativePathHash);
        }
    }
}
