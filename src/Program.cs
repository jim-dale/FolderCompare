
namespace FolderCompare
{
    using McMaster.Extensions.CommandLineUtils;

    internal class Program
    {
        private static int Main(string[] args)
        {
            var app = new CommandLineApplication<FolderCompareApp>();
            app.Model.Configure(app);

            var exitCode = app.Execute(args);
            return exitCode;
        }
    }
}
