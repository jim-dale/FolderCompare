
namespace FolderCompare
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class FileMetadataLookup
    {
        private readonly ILookup<string, FileMetadata> _relPathLookup;
        private readonly ILookup<string, FileMetadata> _contentsLookup;

        public FileMetadataLookup(IEnumerable<FileMetadata> items)
        {
            _relPathLookup = items.ToLookup(i => i.RelativePathHash, StringComparer.InvariantCultureIgnoreCase);
            _contentsLookup = items.ToLookup(i => i.ContentsHash, StringComparer.InvariantCultureIgnoreCase);
        }

        public IEnumerable<FileMetadata> FindMatches(FileMetadata item)
        {
            var result = _relPathLookup[item.RelativePathHash];
            if (result.FirstOrDefault() is null)
            {
                result = _contentsLookup[item.ContentsHash];
            }
            return result;
        }
    }
}
