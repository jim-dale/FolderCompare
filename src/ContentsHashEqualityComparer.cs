
namespace FolderCompare
{
    using System.Collections.Generic;

    public class ContentsHashEqualityComparer : EqualityComparer<FileMetadata>
    {
        private readonly IEqualityComparer<string> _comparer = EqualityComparer<string>.Default;

        public override bool Equals(FileMetadata x, FileMetadata y)
        {
            return _comparer.Equals(x?.ContentsHash, y?.ContentsHash);
        }

        public override int GetHashCode(FileMetadata obj)
        {
            return obj.ContentsHash.GetHashCode();
        }
    }
}
