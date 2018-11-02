
namespace FolderCompare
{
    public static class FileMetadataExtensions
    {
        public static FileMetadata HashFileContents(this FileMetadata item)
        {
            item.ContentsHash = HashStream.GetFileHashSHA512(item.OriginalPath);
            return item;
        }
    }
}
