
namespace FolderCompare
{
    using System.Collections.Generic;
    using System.Linq;

    public class CreateContext
    {
        public IMetadataSource Source { get; set; }
        public IMetadataTarget Target { get; set; }
        public bool NoHashContents { get; set; }

        public IEnumerable<FileMetadata> Items { get; set; }

        public int Run()
        {
            Items = Source.GetAll();
            if (Items.Any())
            {
                if (NoHashContents == false)
                {
                    Helpers.GenerateContentsHash(Items, false);
                }

                Target.SaveAll(Items);
            }
            return ExitCode.Okay;
        }
    }
}
