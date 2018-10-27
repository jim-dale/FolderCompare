
namespace FolderCompare
{
    using System;
    using System.Collections.Generic;

    public class FileMetadata : IComparable<FileMetadata>, IEqualityComparer<FileMetadata>
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

        public int CompareTo(FileMetadata other)
        {
            return string.Compare(Hash, other.Hash);
        }

        public bool Equals(FileMetadata x, FileMetadata y)
        {
            return (x.Hash == y.Hash);
        }

        public int GetHashCode(FileMetadata obj)
        {
            return obj.Hash.GetHashCode();
        }
    }
}
