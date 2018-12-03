
namespace FolderCompare
{
    using McMaster.Extensions.CommandLineUtils;
    using System;
    using System.Reflection;

    public class FolderCompareApp
    {
        private CommandLineApplication<FolderCompareApp> _app;

        public CommandLineApplication<FolderCompareApp> Configure(CommandLineApplication<FolderCompareApp> app)
        {
            _app = app;
            _app.Name = "FolderCompare";
            _app.Description = "Compare two folders or folder catalogues.";
            _app.HelpOption("-?|--help");
            _app.VersionOption("--version", app.Model.GetVersion);

            _app.Command<CreateAppCommand>("create", (cmd) => cmd.Model.Configure(cmd));
            _app.Command<DuplicatesAppCommand>("duplicates", (cmd) => cmd.Model.Configure(cmd));
            _app.Command<CompareAppCommand>("compare", (cmd) => cmd.Model.Configure(cmd));

            _app.OnExecute((Func<int>)OnExecute);

            return _app;
        }

        public string GetVersion()
        {
            return typeof(FolderCompareApp)
                .Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion;
        }

        protected int OnExecute()
        {
            Console.WriteLine("Specify a subcommand");
            Console.WriteLine();

            _app.ShowHelp();

            return ExitCode.ErrorInCommandLine;
        }
    }
}
