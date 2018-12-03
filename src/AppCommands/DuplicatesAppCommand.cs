
namespace FolderCompare
{
    using System;
    using McMaster.Extensions.CommandLineUtils;

    public class DuplicatesAppCommand
    {
        public CommandOption InputOption { get; private set; }

        public void Configure(CommandLineApplication<DuplicatesAppCommand> cmd)
        {
            cmd.HelpOption("-?|--help");

            InputOption = cmd.Option("-i|--input <PATH>", "Path to the folder or catalogue to search for duplicate files.", CommandOptionType.SingleValue)
                .IsRequired()
                .Accepts(v => v.ExistingFileOrDirectory());

            cmd.OnExecute((Func<int>)OnExecute);
        }

        private int OnExecute()
        {
            var context = new DuplicatesCommand
            {
                Source = Helpers.GetMetadataSource(Helpers.ExpandPath(InputOption.Value())),
            };

            return context.Run();
        }
    }
}
