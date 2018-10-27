
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
        public CommandOption<DisplayType> DisplayType { get; private set; }

        public CompareContext Context { get; private set; }

        public void Configure(CommandLineApplication<CompareCommand> cmd)
        {
            LeftPath = cmd.Option("-l|--left", "The folder to search.", CommandOptionType.SingleValue)
                .IsRequired()
                .Accepts(v => v.LegalFilePath());

            RightPath = cmd.Option<string>("-r|--right", "Output JSON catalogue file", CommandOptionType.SingleValue)
                .IsRequired()
                .Accepts(v => v.LegalFilePath());

            DisplayType = cmd.Option<DisplayType>("-d|--displayType", "Display type. None, LeftOnly, RightOnly, Differences or All", CommandOptionType.SingleValue)
                .Accepts(v => v.Enum<DisplayType>(true));

            cmd.OnExecute((Func<int>)OnExecute);
        }

        private int OnExecute()
        {
            Context = new CompareContext
            {
                LeftSource = Helpers.GetMetadataSource(Helpers.ExpandPath(LeftPath.Value())),
                RightSource = Helpers.GetMetadataSource(Helpers.ExpandPath(RightPath.Value())),
                Comparer = Comparer<FileMetadata>.Default,
                EqualityComparer = EqualityComparer<FileMetadata>.Default,
                OutputType = DisplayType.ParsedValue,
            };

            Context.LeftItems= Context.LeftSource.GetAll();
            Context.RightItems = Context.RightSource.GetAll();

            int cmp = 0;
            var items = Helpers.GetFullOuterJoin(Context.LeftItems, Context.RightItems, Context.EqualityComparer);

            if (items.Any())
            {
                Console.WriteLine(Helpers.GetPathsAsTableRow(Console.WindowWidth, Context.LeftSource.Source, Context.RightSource.Source));

                foreach (var item in items)
                {
                    cmp |= Context.Comparer.Compare(item.Item1, item.Item2);

                    Helpers.ShowDifferenceResult(item.Item1, item.Item2, Context.OutputType);
                }
            }
            return Helpers.GetComparisionResultAsExitCode(cmp);
        }
    }
}
