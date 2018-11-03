﻿
namespace FolderCompare
{
    using System;
    using System.Linq;
    using McMaster.Extensions.CommandLineUtils;

    public class CreateCommand
    {
        public CommandOption SourcePath { get; private set; }
        public CommandOption TargetPath { get; private set; }

        public CreateContext Context { get; private set; }

        public void Configure(CommandLineApplication<CreateCommand> cmd)
        {
            SourcePath = cmd.Option("-i|--input <PATH>", "Path to the folder to search.", CommandOptionType.SingleValue)
                .IsRequired()
                .Accepts(v => v.ExistingDirectory());

            TargetPath = cmd.Option("-o|--output <PATH>", "Path to the JSON catalogue file to create.", CommandOptionType.SingleValue)
                .IsRequired()
                .Accepts(v => v.LegalFilePath());

            cmd.OnExecute((Func<int>)OnExecute);
        }

        private int OnExecute()
        {
            Context = new CreateContext
            {
                Source = Helpers.GetMetadataSource(Helpers.ExpandPath(SourcePath.Value())),
                Target = Helpers.GetMetadataTarget(Helpers.ExpandPath(TargetPath.Value())),
            };

            Context.Items = Context.Source.GetAll();
            if (Context.Items.Any())
            {
                Context.Target.SaveAll(Context.Items);
            }
            return ExitCode.Okay;
        }
    }
}