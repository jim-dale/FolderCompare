
namespace FolderCompare
{
    using System;
    using System.Text;
    using McMaster.Extensions.CommandLineUtils;

    internal class Program
    {
        private static int Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;

            var app = new CommandLineApplication<FolderCompareApp>();
            app.Model.Configure(app);

            return app.Execute(args);
        }
    }
}
