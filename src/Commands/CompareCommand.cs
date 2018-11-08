
namespace FolderCompare
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using McMaster.Extensions.CommandLineUtils;

    public class CompareCommand
    {
        public CommandOption LeftPath { get; private set; }
        public CommandOption RightPath { get; private set; }
        public CommandOption CompareContentsHash { get; private set; }
        public CommandOption<DisplayMode> Display { get; private set; }

        public CompareContext Context { get; private set; }

        public void Configure(CommandLineApplication<CompareCommand> cmd)
        {
            LeftPath = cmd.Option("-l|--left <PATH>", "The left folder or catalogue to compare.", CommandOptionType.SingleValue)
                .IsRequired()
                .Accepts(v => v.LegalFilePath());

            RightPath = cmd.Option("-r|--right <PATH>", "The right folder or catalogue to compare.", CommandOptionType.SingleValue)
                .IsRequired()
                .Accepts(v => v.LegalFilePath());

            CompareContentsHash = cmd.Option("-cch|--compare-contents-hash", "", CommandOptionType.NoValue);

            Display = cmd.Option<DisplayMode>("-d|--displayMode <MODE>", Helpers.DisplayModeNamesAsString(), CommandOptionType.SingleValue)
                .Accepts(v => v.Enum<DisplayMode>(true));

            cmd.OnExecute((Func<int>)OnExecute);
        }

        private int OnExecute()
        {
            Context = new CompareContext
            {
                LeftSource = Helpers.GetMetadataSource(Helpers.ExpandPath(LeftPath.Value())),
                RightSource = Helpers.GetMetadataSource(Helpers.ExpandPath(RightPath.Value())),
                OutputType = Display.HasValue() ? Display.ParsedValue : DisplayMode.All,

                Comparer = new FileMetadataComparer(),
                EqualityComparer = new RelPathHashEqualityComparer(),
                ContentsComparer = CompareContentsHash.HasValue() ? new ContentsHashEqualityComparer() : default(IEqualityComparer<FileMetadata>),
            };

            Context.Report = new ConsoleComparisonReport(Context.OutputType, Console.WindowWidth);

            Context.LeftItems = Context.LeftSource.GetAll();
            Context.RightItems = Context.RightSource.GetAll();

            int combined = 0;

            var items = Context.LeftItems.FullOuterJoin(Context.RightItems, Context.EqualityComparer);
            if (items.Any())
            {
                Context.Report.SetSources(Context.LeftSource.Source, Context.RightSource.Source);

                foreach (var item in items)
                {
                    int comparison = Context.Comparer.Compare(item.Item1, item.Item2);
                    combined |= comparison;

                    bool? areEqual = null;
                    if (item.Item1?.ContentsHash != null && item.Item2?.ContentsHash != null && Context.ContentsComparer != null)
                    {
                        areEqual = Context.ContentsComparer.Equals(item.Item1, item.Item2);
                    }

                    Context.Report.OutputRow(item.Item1, item.Item2, comparison, areEqual);
                }
            }

            return Helpers.GetComparisonResultAsExitCode(combined);
        }
    }
}
