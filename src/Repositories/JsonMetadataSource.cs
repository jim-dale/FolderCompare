
namespace FolderCompare
{
    using System.Collections.Generic;

    public class JsonMetadataSource : IMetadataSource
    {
        public string Source { get; set; }

        public List<FileMetadata> GetAll()
        {
            return Helpers.LoadFromJson(Source);
        }
    }
}
