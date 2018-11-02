
namespace FolderCompare
{
    using System.Collections.Generic;

    public class HashEqualityComparer : EqualityComparer<FileMetadata>
    {
        public override bool Equals(FileMetadata x, FileMetadata y)
        {
            return (x.Hash == y.Hash);
        }

        public override int GetHashCode(FileMetadata obj)
        {
            return obj.Hash.GetHashCode();
        }
    }
}
