
namespace FolderCompare
{
    using System;
    using System.Linq;
    using McMaster.Extensions.CommandLineUtils;

    public class ChangesCommand
    {
        private const DisplayMode DefaultDisplayMode = DisplayMode.All;

        public CommandOption LeftPathOption { get; private set; }
        public CommandOption RightPathOption { get; private set; }
        public CommandOption<DisplayMode> DisplayModeOption { get; private set; }

        public void Configure(CommandLineApplication<ChangesCommand> cmd)
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
            var changes = new ChangesContext();

            var leftSource = Helpers.GetMetadataSource(Helpers.ExpandPath(LeftPathOption.Value()));
            var rightSource = Helpers.GetMetadataSource(Helpers.ExpandPath(RightPathOption.Value()));
            var displayMode = DisplayModeOption.HasValue() ? DisplayModeOption.ParsedValue : DefaultDisplayMode;

            var exitCode = ExitCode.FoldersAreTheSame;
            var report = new ConsoleComparisonReport(displayMode);

            var items = changes.GetItems(leftSource, rightSource);
            if (items.Any())
            {
                exitCode = ExitCode.FoldersAreDifferent;

                report.SetSources(leftSource.Source, rightSource.Source);

                foreach (var item in items)
                {
                    report.OutputRow(item);
                }
            }

            return exitCode;
        }
    }
}
