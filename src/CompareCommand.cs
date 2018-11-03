
namespace FolderCompare
{
    using System;
    using System.Linq;
    using McMaster.Extensions.CommandLineUtils;

    public class CompareCommand
    {
        public CommandOption LeftPath { get; private set; }
        public CommandOption RightPath { get; private set; }
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

            var s = String.Join(", ", Enum.GetNames(typeof(DisplayMode)));
            Display = cmd.Option<DisplayMode>("-d|--displayMode <MODE>", s, CommandOptionType.SingleValue)
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
            };

            Context.Report = new ComparisonReport(Context.OutputType, Console.WindowWidth);

            Context.LeftItems = Context.LeftSource.GetAll();
            Context.RightItems = Context.RightSource.GetAll();

            int combined = 0;

            var items = Context.LeftItems.FullOuterJoin(Context.RightItems, Context.EqualityComparer);
            if (items.Any())
            {
                Context.Report.OutputHeader(Context.LeftSource.Source, Context.RightSource.Source);

                foreach (var item in items)
                {
                    int comparison = Context.Comparer.Compare(item.Item1, item.Item2);

                    Context.Report.OutputRow(item.Item1, item.Item2, comparison);

                    combined |= comparison;
                }
            }

            return Helpers.GetComparisonResultAsExitCode(combined);
        }
    }
}
