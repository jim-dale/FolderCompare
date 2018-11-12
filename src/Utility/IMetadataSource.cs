
namespace FolderCompare
{
    using System.Collections.Generic;

    public interface IMetadataSource
    {
        string Source { get; set; }
        List<FileMetadata> GetAll();
    }
}
