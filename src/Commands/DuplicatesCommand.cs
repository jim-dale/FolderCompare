
namespace FolderCompare
{
    using McMaster.Extensions.CommandLineUtils;
    using System;
    using System.Linq;

    public class DuplicatesCommand
    {
        private const DisplayMode DefaultDisplayMode = DisplayMode.All;

        public CommandOption InputOption { get; private set; }

        public DuplicatesContext Context { get; private set; }

        public void Configure(CommandLineApplication<DuplicatesCommand> cmd)
        {
            cmd.HelpOption("-?|--help");

            InputOption = cmd.Option("-i|--input <PATH>", "Path to the folder or catalogue to search for duplicate files.", CommandOptionType.SingleValue)
                .IsRequired()
                .Accepts(v => v.ExistingFileOrDirectory());

            cmd.OnExecute((Func<int>)OnExecute);
        }

        private int OnExecute()
        {
            Context = new DuplicatesContext
            {
                Source = Helpers.GetMetadataSource(Helpers.ExpandPath(InputOption.Value())),
            };

            Context.Report = new ConsoleComparisonReport(DisplayMode.All);

            Context.Items = Context.Source.GetAll();

            var duplicates = Context.Items.GetDuplicates(i => i.ContentsHash);
            foreach (var group in duplicates)
            {
                Console.WriteLine(group.First().FileName);
                foreach (var item in group)
                {
                    Console.WriteLine($"\t{item.RelativePath}");
                }
            }

            return ExitCode.Okay;
        }
    }
}
