
namespace FolderCompare
{
    using System.Collections.Generic;

    public interface IMetadataTarget
    {
        string Target { get; set; }
        void SaveAll(IEnumerable<FileMetadata> items);
    }
}
