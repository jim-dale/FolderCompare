
namespace FolderCompare
{
    using System;
    using McMaster.Extensions.CommandLineUtils;

    public class CompareAppCommand
    {
        private const DisplayMode DefaultDisplayMode = DisplayMode.All;

        public CommandOption LeftPathOption { get; private set; }
        public CommandOption RightPathOption { get; private set; }
        public CommandOption<DisplayMode> DisplayModeOption { get; private set; }

        public void Configure(CommandLineApplication<CompareAppCommand> cmd)
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

            cmd.OnExecute((Func<int>)OnExecute);
        }

        private int OnExecute()
        {
            var context = new CompareCommand
            {
                LeftSource = Helpers.GetMetadataSource(Helpers.ExpandPath(LeftPathOption.Value())),
                RightSource = Helpers.GetMetadataSource(Helpers.ExpandPath(RightPathOption.Value())),
                DisplayMode = DisplayModeOption.HasValue() ? DisplayModeOption.ParsedValue : DefaultDisplayMode,
            };

            return context.Run();
        }
    }
}
