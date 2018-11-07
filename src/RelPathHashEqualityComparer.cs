﻿
namespace FolderCompare
{
    using System.Collections.Generic;

    public class RelPathHashEqualityComparer : EqualityComparer<FileMetadata>
    {
        private readonly IEqualityComparer<string> _comparer = EqualityComparer<string>.Default;

        public override bool Equals(FileMetadata x, FileMetadata y)
        {
            return _comparer.Equals(x?.RelativePathHash, y?.RelativePathHash);
        }

        public override int GetHashCode(FileMetadata obj)
        {
            return obj.RelativePathHash.GetHashCode();
        }
    }
}
