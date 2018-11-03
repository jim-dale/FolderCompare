
namespace FolderCompare
{
    using System.Collections.Generic;

    public class ContentsHashEqualityComparer : EqualityComparer<FileMetadata>
    {
        public override bool Equals(FileMetadata x, FileMetadata y)
        {
            return (x.ContentsHash.Equals(y.ContentsHash));
        }

        public override int GetHashCode(FileMetadata obj)
        {
            return obj.ContentsHash.GetHashCode();
        }
    }
}
