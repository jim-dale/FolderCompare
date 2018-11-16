
namespace FolderCompare
{
    using McMaster.Extensions.CommandLineUtils;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ChangesContext
    {
        // Set by command line parameters
        public IMetadataSource LeftSource { get; set; }
        public IMetadataSource RightSource { get; set; }
        public DisplayMode DisplayMode { get; set; }
        public ContentsMode ContentsMode { get; set; }

        // Set at runtime
        public IComparer<FileMetadata> Comparer { get; set; }
        public IEqualityComparer<FileMetadata> EqualityComparer { get; set; }
        public IEqualityComparer<FileMetadata> ContentsComparer { get; set; }

        public IComparisonReport Report { get; set; }

        public IEnumerable<FileMetadata> LeftItems { get; set; }
        public IEnumerable<FileMetadata> RightItems { get; set; }
    }

    public class ChangesCommand
    {
        private const DisplayMode DefaultDisplayMode = DisplayMode.All;
        private const ContentsMode DefaultContentsMode = ContentsMode.All;

        public CommandOption LeftPathOption { get; private set; }
        public CommandOption RightPathOption { get; private set; }
        public CommandOption<DisplayMode> DisplayModeOption { get; private set; }
        public CommandOption<ContentsMode> ContentsModeOption { get; private set; }

        public ChangesContext Context { get; private set; }

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
            Context = new ChangesContext
            {
                LeftSource = Helpers.GetMetadataSource(Helpers.ExpandPath(LeftPathOption.Value())),
                RightSource = Helpers.GetMetadataSource(Helpers.ExpandPath(RightPathOption.Value())),
                DisplayMode = DisplayModeOption.HasValue() ? DisplayModeOption.ParsedValue : DefaultDisplayMode,
                ContentsMode = ContentsModeOption.HasValue() ? ContentsModeOption.ParsedValue : DefaultContentsMode,

                Comparer = new FileMetadataComparer(),
                EqualityComparer = new RelPathHashEqualityComparer(),
                ContentsComparer = new ContentsHashEqualityComparer(),
            };

            Context.Report = new ConsoleComparisonReport(Context.DisplayMode, Context.ContentsMode);

            Context.LeftItems = Context.LeftSource.GetAll();
            Context.RightItems = Context.RightSource.GetAll();

            var exitCode = ExitCode.FoldersAreTheSame;

            // Exists on both sides but different FileMetadata
            // Moved RelPath different but ContentsHash the same (excluding duplicates)
            // Duplicates by ContentsHash
            // Orphan - exists only on one side

            // Exclude items that are the same in left and right lists
            IEqualityComparer<FileMetadata> equalityComparer = new FileMetadataEqualityComparer();
            var leftItems = Context.LeftItems.Except(Context.RightItems, equalityComparer);
            var rightItems = Context.RightItems.Except(Context.LeftItems, equalityComparer);

            // Create lookups to speed up matching items from left to right
            var rightRelPathLookup = rightItems.ToLookup(i => i.RelativePathHash);
            var rightContentsLookup = rightItems.ToLookup(i => i.ContentsHash);

            List<FileMetadata> rightConsumed = new List<FileMetadata>();
            List<CompareViewModel> items = new List<CompareViewModel>();

            foreach (var leftItem in leftItems)
            {
                var rightItem = default(FileMetadata);

                var matches = rightRelPathLookup[leftItem.RelativePathHash];
                if (matches.Count() == 1)
                {
                    rightItem = matches.Single();
                }
                else
                {
                    var contentsMatches = rightContentsLookup[leftItem.ContentsHash];
                    if (contentsMatches.Count() == 1)
                    {
                        rightItem = contentsMatches.Single();
                    }
                }
                if (rightItem != null)
                {
                    rightConsumed.Add(rightItem);
                }
                items.Add(Helpers.CreateViewModel(leftItem, rightItem, Context.Comparer, Context.ContentsComparer));
            }
            var rightRemaining = rightItems.Except(rightConsumed);
            foreach (var rightItem in rightRemaining)
            {
                items.Add(Helpers.CreateViewModel(null, rightItem, Context.Comparer, Context.ContentsComparer));
            }

            if (items.Any())
            {
                exitCode = ExitCode.FoldersAreDifferent;

                Context.Report.SetSources(Context.LeftSource.Source, Context.RightSource.Source);

                foreach (var item in items)
                {
                    Context.Report.OutputRow(item);
                }
            }

            return exitCode;
        }
    }
}
