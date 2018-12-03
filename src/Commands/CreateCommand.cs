
namespace FolderCompare
{
    using System.Linq;

    public class CreateCommand : ICommand
    {
        public IMetadataSource Source { get; set; }
        public IMetadataTarget Target { get; set; }
        public bool NoHashContents { get; set; }

        public int Run()
        {
            var items = Source.GetAll();
            if (items.Any())
            {
                if (NoHashContents == false)
                {
                    Helpers.GenerateContentsHash(items, false);
                }

                Target.SaveAll(items);
            }
            return ExitCode.Okay;
        }
    }
}
