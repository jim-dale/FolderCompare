
namespace FolderCompare
{
    using System.Collections.Generic;

    public class JsonMetadataTarget : IMetadataTarget
    {
        public string Target { get; set; }

        public void SaveAll(IEnumerable<FileMetadata> items)
        {
            Helpers.SaveToJson(items, Target);
        }
    }
}
