
namespace FolderCompare
{
    using System;
    using System.Linq;

    public class DuplicatesCommand : ICommand
    {
        public IMetadataSource Source { get; set; }

        public int Run()
        {
            var items = Source.GetAll();

            var duplicates = items.GetDuplicates(i => i.ContentsHash, StringComparer.InvariantCultureIgnoreCase);
            foreach (var group in duplicates)
            {
                Console.WriteLine(group.First().FileName);
                foreach (var item in group)
                {
                    Console.WriteLine($"\t{item.RelativePath}");
                }
            }

            return ExitCode.Okay;
        }
    }
}
