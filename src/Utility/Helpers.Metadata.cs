
namespace FolderCompare
{
    using System.IO;

    public static partial class Helpers
    {
        private const string JsonExtension = ".json";

        public static IMetadataSource GetMetadataSource(string path)
        {
            var result = default(IMetadataSource);

            if (Directory.Exists(path))
            {
                result = new DirectoryMetadataSource { Source = path };
            }
            else if (File.Exists(path) && Path.GetExtension(path) == JsonExtension)
            {
                result = new JsonMetadataSource { Source = path };
            }
            return result;
        }

        public static IMetadataTarget GetMetadataTarget(string path)
        {
            var result = default(IMetadataTarget);

            if (Path.GetExtension(path) == JsonExtension)
            {
                result = new JsonMetadataTarget { Target = path };
            }
            return result;
        }
    }
}
