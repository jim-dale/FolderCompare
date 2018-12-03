
namespace FolderCompare
{
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;

    public static partial class Helpers
    {
        public static List<FileMetadata> LoadFromJson(string path)
        {
            string json = File.ReadAllText(path);

            return JsonConvert.DeserializeObject<List<FileMetadata>>(json);
        }

        public static void SaveToJson(IEnumerable<FileMetadata> items, string path)
        {
            string json = JsonConvert.SerializeObject(items, Formatting.Indented);
            File.WriteAllText(path, json);
        }
    }
}
