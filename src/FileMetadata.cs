
namespace FolderCompare
{
    using System;
    using System.Collections.Generic;

    public class HashComparer : Comparer<FileMetadata>
    {
        public override int Compare(FileMetadata x, FileMetadata y)
        {
            return string.Compare(x.Hash, y.Hash);
        }
    }

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

    public class FileMetadata
    {
        public string FileName { get; internal set; }
        public long Length { get; set; }
        public DateTime LastAccessTimeUtc { get; set; }
        public DateTime CreationTimeUtc { get; set; }
        public DateTime LastWriteTimeUtc { get; set; }
        public string OriginalPath { get; set; }
        public string RelativePath { get; set; }
        public string FileHash { get; set; }
        public string PathHash { get; set; }
        public string Hash { get; set; }
    }
}
