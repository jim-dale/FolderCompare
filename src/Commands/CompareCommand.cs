
namespace FolderCompare
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public class CompareCommand : ICommand
    {
        public IMetadataSource LeftSource { get; set; }
        public IMetadataSource RightSource { get; set; }
        public DisplayMode DisplayMode { get; set; }

        public int Run()
        {
            var exitCode = ExitCode.FoldersAreTheSame;
            var report = new ConsoleComparisonReport(DisplayMode);

            var leftItems = LeftSource.GetAll();
            var rightItems = RightSource.GetAll();

            var items = GetItems(leftItems, rightItems);
            if (items.Any())
            {
                exitCode = ExitCode.FoldersAreDifferent;

                report.SetSources(LeftSource.Source, RightSource.Source);

                foreach (var item in items)
                {
                    report.OutputRow(item);
                }
            }

            return exitCode;
        }

        /// <summary>
        /// Exists on both sides but different FileMetadata
        /// Moved RelPath different but ContentsHash the same (excluding duplicates)
        /// Duplicates by ContentsHash
        /// Orphan - exists only on one side
        /// </summary>
        /// <param name="leftSource"></param>
        /// <param name="rightSource"></param>
        /// <returns></returns>
        public IEnumerable<CompareViewModel> GetItems(IEnumerable<FileMetadata> leftItems, IEnumerable<FileMetadata> rightItems)
        {
            var pairs = MatchPairs(leftItems, rightItems);

#if DEBUG
            foreach (var leftItem in leftItems)
            {
                var item = pairs.SingleOrDefault(i => i.LeftItem == leftItem);
                if (item is null)
                {
                    Trace.TraceInformation($"Left Item not mapped = \"{leftItem.RelativePath}\"");
                }
            }
            foreach (var rightItem in rightItems)
            {
                var item = pairs.SingleOrDefault(i => i.RightItem == rightItem);
                if (item is null)
                {
                    Trace.TraceInformation($"Right Item not mapped = \"{rightItem.RelativePath}\"");
                }
            }
#endif

            var query = from item in pairs
                        select ViewModelFactory.Create(item.LeftItem, item.RightItem);

            var result = query.OrderBy((vm) =>
            {
                var res = vm.LeftItem?.RelativePath ?? vm.RightItem?.RelativePath;
                return res;
            }, StringComparer.InvariantCultureIgnoreCase);

            return result;
        }

        internal List<Pair> MatchPairs(IEnumerable<FileMetadata> leftItems, IEnumerable<FileMetadata> rightItems)
        {
            var results = new List<Pair>();

            var notMatched = MatchByRelPathHash(leftItems, rightItems, results);

            var stillNotMatched = MatchByContentsHash(notMatched, results);

            foreach (var rightItem in stillNotMatched)
            {
                results.Add(Pair.Create(null, rightItem));
            }

            return results;
        }

        /// <summary>
        /// Performs a left outer join of <paramref name="leftItems"  /> to <paramref name="rightItems"/> and stores the result set in <paramref name="results"/>.
        /// </summary>
        /// <param name="leftItems"></param>
        /// <param name="rightItems"></param>
        /// <param name="results"></param>
        /// <returns>The list of items from <paramref name="rightItems"/> that were not matched to items from <paramref name="leftItems"/></returns>
        private static IList<FileMetadata> MatchByRelPathHash(IEnumerable<FileMetadata> leftItems, IEnumerable<FileMetadata> rightItems, IList<Pair> results)
        {
            var matched = new List<FileMetadata>();
            var index = rightItems.ToDictionary(i => i.RelativePathHash, StringComparer.InvariantCultureIgnoreCase);

            foreach (var leftItem in leftItems)
            {
                var rightItem = default(FileMetadata);

                if (index.TryGetValue(leftItem.RelativePathHash, out FileMetadata item))
                {
                    if (matched.Contains(item) == false)
                    {
                        matched.Add(item);
                        rightItem = item;
                    }
                }

                results.Add(Pair.Create(leftItem, rightItem));
            }

            return rightItems.Except(matched).ToList();
        }

        /// <summary>
        /// Performs a right outer join of <paramref name="rightItems"  /> to <paramref name="results"/> and stores the result set in <paramref name="results"/>.
        /// </summary>
        /// <param name="rightItems"></param>
        /// <param name="results"></param>
        /// <returns>The list of items from <paramref name="rightItems"/> that were not matched to items from <paramref name="results"/></returns>
        private static IList<FileMetadata> MatchByContentsHash(IEnumerable<FileMetadata> rightItems, IEnumerable<Pair> results)
        {
            var matched = new List<FileMetadata>();

            foreach (var rightItem in rightItems)
            {
                var matches = from i in results
                              where i.RightItem is null && StringComparer.InvariantCultureIgnoreCase.Equals(i.LeftItem.ContentsHash, rightItem.ContentsHash)
                              select i;
                if (matches.Count() == 1)
                {
                    var item = matches.Single();

                    if (matched.Contains(rightItem) == false)
                    {
                        matched.Add(rightItem);
                        item.RightItem = rightItem;
                    }
                }
            }

            return rightItems.Except(matched).ToList();
        }
    }
}
