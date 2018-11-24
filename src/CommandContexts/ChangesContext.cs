
namespace FolderCompare
{
    using System.Collections.Generic;
    using System.Linq;

    public class ChangesContext
    {
        /// <summary>
        /// Exists on both sides but different FileMetadata
        /// Moved RelPath different but ContentsHash the same (excluding duplicates)
        /// Duplicates by ContentsHash
        /// Orphan - exists only on one side
        /// </summary>
        /// <param name="leftSource"></param>
        /// <param name="rightSource"></param>
        /// <returns></returns>

        public IEnumerable<CompareViewModel> GetItems(IMetadataSource leftSource, IMetadataSource rightSource)
        {
            var leftItems = leftSource.GetAll();
            var rightItems = rightSource.GetAll();

            // Create lookup to speed up matching items from left to right
            var lookup = new FileMetadataLookup(rightItems);

            List<FileMetadata> rightMatched = new List<FileMetadata>();
            List<CompareViewModel> items = new List<CompareViewModel>();

            foreach (var leftItem in leftItems)
            {
                var rightItem = default(FileMetadata);

                var matches = lookup.FindMatches(leftItem);
                if (matches.Count() == 1)
                {
                    rightItem = matches.Single();
                }
                if (rightItem != null)
                {
                    rightMatched.Add(rightItem);
                }
                items.Add(Helpers.CreateViewModel(leftItem, rightItem));
            }
            var rightRemaining = rightItems.Except(rightMatched);
            foreach (var rightItem in rightRemaining)
            {
                items.Add(Helpers.CreateViewModel(null, rightItem));
            }

            return items;
        }
    }
}
