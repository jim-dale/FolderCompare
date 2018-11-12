
namespace FolderCompare
{
    using System;
    using System.Linq;
    using McMaster.Extensions.CommandLineUtils;

    public class CreateCommand
    {
        public CommandOption SourcePathOption { get; private set; }
        public CommandOption TargetPathOption { get; private set; }
        public CommandOption NoHashContentsOption { get; private set; }

        public CreateContext Context { get; private set; }

        public void Configure(CommandLineApplication<CreateCommand> cmd)
        {
            cmd.HelpOption("-?|--help");

            SourcePathOption = cmd.Option("-i|--input <PATH>", "Path to the folder to search.", CommandOptionType.SingleValue)
                .IsRequired()
                .Accepts(v => v.ExistingDirectory());

            TargetPathOption = cmd.Option("-o|--output <PATH>", "Path to the JSON catalogue file to create.", CommandOptionType.SingleValue)
                .IsRequired()
                .Accepts(v => v.LegalFilePath());

            NoHashContentsOption = cmd.Option("--no-hash-contents", "Do not generate a hash of the contents of each file.", CommandOptionType.NoValue);

            cmd.OnExecute((Func<int>)OnExecute);
        }

        private int OnExecute()
        {
            Context = new CreateContext
            {
                Source = Helpers.GetMetadataSource(Helpers.ExpandPath(SourcePathOption.Value())),
                Target = Helpers.GetMetadataTarget(Helpers.ExpandPath(TargetPathOption.Value())),
                NoHashContents = NoHashContentsOption.HasValue(),
            };

            Context.Items = Context.Source.GetAll();
            if (Context.Items.Any())
            {
                if (Context.NoHashContents == false)
                {
                    Helpers.GenerateContentsHash(Context.Items, false);
                }

                Context.Target.SaveAll(Context.Items);
            }
            return ExitCode.Okay;
        }
    }
}
