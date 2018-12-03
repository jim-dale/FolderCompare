
namespace FolderCompare
{
    using System.IO;

    public static class FileMetadataFactory
    {
        public static FileMetadata Create(DirectoryInfo directoryInfo, FileInfo fileInfo)
        {
            var rp = directoryInfo.GetRelativePathTo(fileInfo);

            var result = new FileMetadata
            {
                FileName = fileInfo.Name,
                Length = fileInfo.Length,
                CreationTimeUtc = fileInfo.CreationTimeUtc,
                LastAccessTimeUtc = fileInfo.LastAccessTimeUtc,
                LastWriteTimeUtc = fileInfo.LastWriteTimeUtc,
                OriginalPath = fileInfo.FullName,
                RelativePath = rp,
                RelativePathHash = Helpers.GetStringHashSHA512(rp.ToLowerInvariant()),
            };

            return result;
        }
    }
}
