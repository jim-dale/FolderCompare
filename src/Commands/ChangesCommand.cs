
namespace FolderCompare
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using McMaster.Extensions.CommandLineUtils;

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

    public class Changes
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


    public class ChangesCommand
    {
        private const DisplayMode DefaultDisplayMode = DisplayMode.All;
        private const ContentsMode DefaultContentsMode = ContentsMode.All;

        public CommandOption LeftPathOption { get; private set; }
        public CommandOption RightPathOption { get; private set; }
        public CommandOption<DisplayMode> DisplayModeOption { get; private set; }
        public CommandOption<ContentsMode> ContentsModeOption { get; private set; }

        public void Configure(CommandLineApplication<ChangesCommand> cmd)
        {
            cmd.HelpOption("-?|--help");

            LeftPathOption = cmd.Option("-l|--left <PATH>", "The left folder or catalogue to compare.", CommandOptionType.SingleValue)
                .IsRequired()
                .Accepts(v => v.LegalFilePath());

            RightPathOption = cmd.Option("-r|--right <PATH>", "The right folder or catalogue to compare.", CommandOptionType.SingleValue)
                .IsRequired()
                .Accepts(v => v.LegalFilePath());

            DisplayModeOption = cmd.Option<DisplayMode>("-d|--display-mode <MODE>", Helpers.EnumNamesAsString(DefaultDisplayMode), CommandOptionType.SingleValue)
                .Accepts(v => v.Enum<DisplayMode>(true));

            ContentsModeOption = cmd.Option<ContentsMode>("-c|--contents-mode <MODE>", Helpers.EnumNamesAsString(DefaultContentsMode), CommandOptionType.SingleValue)
                .Accepts(v => v.Enum<ContentsMode>(true));

            cmd.OnExecute((Func<int>)OnExecute);
        }

        private int OnExecute()
        {
            var changes = new Changes();

            var leftSource = Helpers.GetMetadataSource(Helpers.ExpandPath(LeftPathOption.Value()));
            var rightSource = Helpers.GetMetadataSource(Helpers.ExpandPath(RightPathOption.Value()));
            var displayMode = DisplayModeOption.HasValue() ? DisplayModeOption.ParsedValue : DefaultDisplayMode;
            var contentsMode = ContentsModeOption.HasValue() ? ContentsModeOption.ParsedValue : DefaultContentsMode;

            var exitCode = ExitCode.FoldersAreTheSame;
            var report = new ConsoleComparisonReport(displayMode, contentsMode);

            var items = changes.GetItems(leftSource, rightSource);
            if (items.Any())
            {
                exitCode = ExitCode.FoldersAreDifferent;

                report.SetSources(leftSource.Source, rightSource.Source);

                foreach (var item in items)
                {
                    report.OutputRow(item);
                }
            }

            return exitCode;
        }
    }
}
