
namespace FolderCompare
{
    public static class FileMetadataExtensions
    {
        public static FileMetadata HashFileContents(this FileMetadata item)
        {
            item.ContentsHash = Helpers.GetFileHashSHA512(item.OriginalPath);
            return item;
        }
    }
}
