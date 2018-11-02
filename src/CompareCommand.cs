
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
            var mode = CompareMode.Quick;

            var dateComparer = Comparer<DateTime>.Default;
            var lengthComparer = Comparer<long>.Default;

            Context = new CompareContext
            {
                LeftSource = Helpers.GetMetadataSource(Helpers.ExpandPath(LeftPath.Value())),
                RightSource = Helpers.GetMetadataSource(Helpers.ExpandPath(RightPath.Value())),
                Comparer = new FileMetadataComparer(),
                EqualityComparer = new HashEqualityComparer(),
                OutputType = Display.HasValue() ? Display.ParsedValue : DisplayMode.All,
                Mode = mode
            };

            Context.LeftItems = Context.LeftSource.GetAll();
            Context.RightItems = Context.RightSource.GetAll();

            int combined = 0;
            var items = Helpers.GetFullOuterJoin(Context.LeftItems, Context.RightItems, Context.EqualityComparer);

            if (items.Any())
            {
                Console.WriteLine(Helpers.GetPathsAsTableRow(Console.WindowWidth, Context.LeftSource.Source, Context.RightSource.Source));

                foreach (var item in items)
                {
                    int cmp = Context.Comparer.Compare(item.Item1, item.Item2);

                    Helpers.ShowDifferenceResult(item.Item1, item.Item2, Context.OutputType, cmp);

                    combined |= cmp;
                }
            }
            return Helpers.GetComparisionResultAsExitCode(combined);
        }
    }
}
