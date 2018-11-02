
namespace FolderCompare
{
    using System;
    using System.Reflection;
    using McMaster.Extensions.CommandLineUtils;

    public class FolderCompareApp
    {
        public CommandLineApplication<FolderCompareApp> Configure(CommandLineApplication<FolderCompareApp> app)
        {
            app.Command<CreateCommand>("create", (cmd) => cmd.Model.Configure(cmd));
            app.Command<CompareCommand>("compare", (cmd) => cmd.Model.Configure(cmd));

            app.VersionOption("--version", app.Model.GetVersion);

            return app;
        }

        public string GetVersion()
        {
            return typeof(FolderCompareApp)
                .Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion;
        }

        protected int OnExecute(CommandLineApplication app)
        {
            Console.WriteLine("Specify a subcommand");

            app.ShowHelp();

            return ExitCode.ErrorInCommandLine;
        }
    }
}
