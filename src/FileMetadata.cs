
namespace FolderCompare
{
    using System;

    public class FileMetadata
    {
        public string FileName { get; set; }
        public long Length { get; set; }
        public DateTime LastAccessTimeUtc { get; set; }
        public DateTime CreationTimeUtc { get; set; }
        public DateTime LastWriteTimeUtc { get; set; }
        public string OriginalPath { get; set; }
        public string RelativePath { get; set; }
        public string RelativePathHash { get; set; }
        public string ContentsHash { get; set; }
    }
}
