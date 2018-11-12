
namespace FolderCompare
{
    using McMaster.Extensions.CommandLineUtils;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CompareCommand
    {
        private const DisplayMode DefaultDisplayMode = DisplayMode.All;
        private const ContentsMode DefaultContentsMode = ContentsMode.All;

        public CommandOption LeftPathOption { get; private set; }
        public CommandOption RightPathOption { get; private set; }
        public CommandOption<DisplayMode> DisplayModeOption { get; private set; }
        public CommandOption<ContentsMode> ContentsModeOption { get; private set; }

        public CompareContext Context { get; private set; }

        public void Configure(CommandLineApplication<CompareCommand> cmd)
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
            Context = new CompareContext
            {
                LeftSource = Helpers.GetMetadataSource(Helpers.ExpandPath(LeftPathOption.Value())),
                RightSource = Helpers.GetMetadataSource(Helpers.ExpandPath(RightPathOption.Value())),
                DisplayMode = DisplayModeOption.HasValue() ? DisplayModeOption.ParsedValue : DefaultDisplayMode,
                ContentsMode = ContentsModeOption.HasValue() ? ContentsModeOption.ParsedValue : DefaultContentsMode,

                Comparer = new FileMetadataComparer(),
                EqualityComparer = new RelPathHashEqualityComparer(),
                ContentsComparer = new ContentsHashEqualityComparer(),
            };

            Context.Report = new ConsoleComparisonReport(Context.DisplayMode, Context.ContentsMode, Console.WindowWidth);

            Context.LeftItems = Context.LeftSource.GetAll();
            Context.RightItems = Context.RightSource.GetAll();

            int combined = 0;

            var items = Join(Context.LeftItems, Context.RightItems, Context.EqualityComparer);
            if (items.Any())
            {
                Context.Report.SetSources(Context.LeftSource.Source, Context.RightSource.Source);

                foreach (var item in items)
                {
                    Context.Report.OutputRow(item);

                    combined |= item.Comparison;
                }
            }

            return Helpers.GetComparisonResultAsExitCode(combined);
        }

        private IEnumerable<CompareViewModel> Join(IEnumerable<FileMetadata> items1, IEnumerable<FileMetadata> items2, IEqualityComparer<FileMetadata> comparer)
        {
            var items = items1.Union(items2, comparer);

            foreach (var item in items)
            {
                var item1 = items1.SingleOrDefault(i => comparer.Equals(i, item));
                var item2 = items2.SingleOrDefault(i => comparer.Equals(i, item));

                int comparison = Context.Comparer.Compare(item1, item2);

                bool? areEqual = null;
                if (item1?.ContentsHash != null && item2?.ContentsHash != null && Context.ContentsComparer != null)
                {
                    areEqual = Context.ContentsComparer.Equals(item1, item2);
                }

                yield return new CompareViewModel
                {
                    LeftItem = item1,
                    RightItem = item2,
                    Comparison = comparison,
                    AreEqual = areEqual
                };
            }
        }
    }
}
