
namespace FolderCompare
{
    using System;
    using Newtonsoft.Json;

    public class FileMetadata
    {
        public string FileName { get; set; }
        public long Length { get; set; }
        public DateTime LastAccessTimeUtc { get; set; }
        public DateTime CreationTimeUtc { get; set; }
        public DateTime LastWriteTimeUtc { get; set; }
        public string OriginalPath { get; set; }
        public string RelativePath { get; set; }
        public string ContentsHash { get; set; }
        public string PathHash { get; set; }

        [JsonIgnore]
        public string Hash { get; set; }
    }
}
