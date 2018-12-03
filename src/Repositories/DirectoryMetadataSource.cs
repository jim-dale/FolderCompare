
namespace FolderCompare
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class DirectoryMetadataSource : IMetadataSource
    {
        public string Source { get; set; }

        public List<FileMetadata> GetAll()
        {
            var source = new DirectoryInfo(Path.GetFullPath(Source));
            return FromDirectoryInfo(source);
        }

        private List<FileMetadata> FromDirectoryInfo(DirectoryInfo directoryInfo)
        {
            var files = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);

            var query = from file in files
                        select FileMetadataFactory.Create(directoryInfo, file);

            return query.ToList();
        }
    }
}
