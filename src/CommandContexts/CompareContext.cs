
namespace FolderCompare
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public partial class CompareContext
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

            var items = GetItems(leftItems, rightItems);

            foreach (var leftItem in leftItems)
            {
                var item = items.SingleOrDefault(i => i.LeftItem == leftItem);
                if (item.LeftItem is null && item.RightItem is null)
                {
                    Console.WriteLine($"Left Item not mapped = \"{leftItem.RelativePath}\"");
                }
            }
            foreach (var rightItem in rightItems)
            {
                var item = items.SingleOrDefault(i => i.RightItem == rightItem);
                if (item.LeftItem is null && item.RightItem is null)
                {
                    Console.WriteLine($"Right Item not mapped = \"{rightItem.RelativePath}\"");
                }
            }

            var query = from item in items
                        select Helpers.CreateViewModel(item.LeftItem, item.RightItem);

            var result = query.OrderBy((vm) =>
            {
                var res = vm.LeftItem?.RelativePath ?? vm.RightItem?.RelativePath;
                return res;
            });
            return result;
        }

        internal List<Pair> GetItems(IEnumerable<FileMetadata> leftItems, IEnumerable<FileMetadata> rightItems)
        {
            var results = new List<Pair>();

            var matchedByRelPathHash = new List<FileMetadata>();
            var lookup = rightItems.ToLookup(i => i.RelativePathHash, StringComparer.InvariantCultureIgnoreCase);

            foreach (var leftItem in leftItems)
            {
                var matches = lookup[leftItem.RelativePathHash];
                var rightItem = default(FileMetadata);
                if (matches.Count() == 1)
                {
                    var item = matches.Single();

                    if (matchedByRelPathHash.Contains(item) == false)
                    {
                        matchedByRelPathHash.Add(item);
                        rightItem = item;
                    }
                }

                results.Add(Pair.Create(leftItem, rightItem));
            }

            var rightRemaining = rightItems.Except(matchedByRelPathHash).ToList();

            var matchedByContentsHash = new List<FileMetadata>();

            foreach (var rightItem in rightRemaining)
            {
                var matches = from i in results
                              where i.RightItem is null && StringComparer.InvariantCultureIgnoreCase.Equals(i.LeftItem.ContentsHash, rightItem.ContentsHash)
                              select i;
                if (matches.Count() == 1)
                {
                    var item = matches.Single();

                    if (matchedByContentsHash.Contains(rightItem) == false)
                    {
                        matchedByContentsHash.Add(rightItem);
                        item.RightItem = rightItem;
                    }
                }
                else if (matches.Any())
                {
                    Trace.TraceWarning($"Multiple matches for {rightItem.ContentsHash}");
                    foreach (var match in matches)
                    {
                        Trace.TraceWarning($"\tRelative file path=\"{match.LeftItem.RelativePath}\"");
                    }
                }
                else
                {
                    Trace.TraceWarning($"No match for {rightItem.RelativePath}");
                }
            }
            var stillRemaining = rightRemaining.Except(matchedByContentsHash).ToList();
            foreach (var rightItem in stillRemaining)
            {
                results.Add(Pair.Create(null, rightItem));
            }

            return results;
        }
    }
}
