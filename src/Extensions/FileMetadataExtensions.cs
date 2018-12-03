
namespace FolderCompare
{
    public static class FileMetadataExtensions
    {
        public static FileMetadata HashFileContents(this FileMetadata item)
        {
            item.ContentsHash = HashHelpers.GetFileHashSHA512(item.OriginalPath);
            return item;
        }
    }
}
