
namespace FolderCompare
{
    using System.Collections.Generic;

    public class FileMetadataEqualityComparer : EqualityComparer<FileMetadata>
    {
        private readonly IComparer<FileMetadata> _comparer = new FileMetadataComparer();

        public override bool Equals(FileMetadata x, FileMetadata y)
        {
            return _comparer.Compare(x, y) == 0;
        }

        public override int GetHashCode(FileMetadata obj)
        {
            return obj.RelativePathHash.GetHashCode();
        }
    }
}
