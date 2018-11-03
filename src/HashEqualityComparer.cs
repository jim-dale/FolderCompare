
namespace FolderCompare
{
    using System.Collections.Generic;

    public class HashEqualityComparer : EqualityComparer<FileMetadata>
    {
        public override bool Equals(FileMetadata x, FileMetadata y)
        {
            return (x.RelativePathHash.Equals(y.RelativePathHash));
        }

        public override int GetHashCode(FileMetadata obj)
        {
            return obj.RelativePathHash.GetHashCode();
        }
    }
}
