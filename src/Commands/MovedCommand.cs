
namespace FolderCompare
{
    using McMaster.Extensions.CommandLineUtils;
    using System;
    using System.Linq;

    /// <summary>
    /// Get items with same ContentsHash
    /// Excude items with same relative path
    /// Exclude items with duplicate content-hash in source
    /// </summary>
    public class MovedCommand
    {
        private const DisplayMode DefaultDisplayMode = DisplayMode.Same;

        public CommandOption LeftPathOption { get; private set; }
        public CommandOption RightPathOption { get; private set; }

        public MovedContext Context { get; private set; }

        public void Configure(CommandLineApplication<MovedCommand> cmd)
        {
            cmd.HelpOption("-?|--help");

            LeftPathOption = cmd.Option("-l|--left <PATH>", "The left folder or catalogue to compare.", CommandOptionType.SingleValue)
                .IsRequired()
                .Accepts(v => v.LegalFilePath());

            RightPathOption = cmd.Option("-r|--right <PATH>", "The right folder or catalogue to compare.", CommandOptionType.SingleValue)
                .IsRequired()
                .Accepts(v => v.LegalFilePath());

            cmd.OnExecute((Func<int>)OnExecute);
        }

        private int OnExecute()
        {
            Context = new MovedContext
            {
                LeftSource = Helpers.GetMetadataSource(Helpers.ExpandPath(LeftPathOption.Value())),
                RightSource = Helpers.GetMetadataSource(Helpers.ExpandPath(RightPathOption.Value())),

                EqualityComparer = new RelPathHashEqualityComparer(),
                ContentsComparer = new ContentsHashEqualityComparer(),

                Report = new ConsoleComparisonReport(DisplayMode.All, ContentsMode.All),
            };

            // Get all metadata entries
            var leftItems = Context.LeftSource.GetAll();
            var rightItems = Context.RightSource.GetAll();

            // Exclude items that appear in both lists
            Context.LeftItems = leftItems.Except(rightItems, Context.EqualityComparer);
            Context.RightItems = rightItems.Except(leftItems, Context.EqualityComparer);

            // Remove items with duplicate ContentsHash value
            Context.LeftItems = Context.LeftItems.RemoveDuplicates(i => i.ContentsHash);
            Context.RightItems = Context.RightItems.RemoveDuplicates(i => i.ContentsHash);

            var items = from leftItem in Context.LeftItems
                        join rightItem in Context.RightItems on leftItem.ContentsHash.ToLowerInvariant() equals rightItem.ContentsHash.ToLowerInvariant()
                        select Helpers.CreateViewModel(leftItem, rightItem);

            if (items.Any())
            {
                Context.Report.SetSources(Context.LeftSource.Source, Context.RightSource.Source);

                foreach (var item in items)
                {
                    Context.Report.OutputRow(item);
                }
            }

            return ExitCode.FoldersAreTheSame;
        }
    }
}
