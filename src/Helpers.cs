
namespace FolderCompare
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public static class Helpers
    {
        public const string JsonExtension = ".json";

        public static string ExpandPath(string path)
        {
            return Environment.ExpandEnvironmentVariables(path);
        }

        public static string DisplayModeNamesAsString()
        {
            return String.Join(", ", Enum.GetNames(typeof(DisplayMode)));
        }

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

        public static FileMetadata CreateFileMetadata(DirectoryInfo directoryInfo, FileInfo fileInfo)
        {
            var rp = directoryInfo.GetRelativePathTo(fileInfo);

            var result = new FileMetadata
            {
                FileName = fileInfo.Name,
                Length = fileInfo.Length,
                CreationTimeUtc = fileInfo.CreationTimeUtc,
                LastAccessTimeUtc = fileInfo.LastAccessTimeUtc,
                LastWriteTimeUtc = fileInfo.LastWriteTimeUtc,
                OriginalPath = fileInfo.FullName,
                RelativePath = rp,
                RelativePathHash = HashStream.GetStringHashSHA512(rp.ToLowerInvariant()),
            };

            return result;
        }

        public static void GenerateHashOfContents(IEnumerable<FileMetadata> items, bool force)
        {
            var query = items;

            if (force == false)
            {
                query = from i in items
                        where i.ContentsHash is null
                        select i;
            }

            foreach (var item in query)
            {
                item.ContentsHash = HashStream.GetFileHashSHA512(item.OriginalPath);
            }
        }

        public static bool GetShouldShowRow(DisplayMode outputType, FileMetadata leftItem, FileMetadata rightItem, int comparison)
        {
            bool result = false;

            switch (outputType)
            {
                case DisplayMode.Same:
                    result = (comparison == 0);
                    break;
                case DisplayMode.LeftOnly:
                    result = (comparison > 0);
                    break;
                case DisplayMode.RightOnly:
                    result = (comparison < 0);
                    break;
                case DisplayMode.Differences:
                    result = (comparison != 0);
                    break;
                case DisplayMode.All:
                    result = true;
                    break;
                case DisplayMode.None:
                default:
                    break;
            }

            return result;
        }

        public static int GetComparisonResultAsExitCode(int cmp)
        {
            return (cmp == 0) ? ExitCode.FoldersAreTheSame : ExitCode.FoldersAreDifferent;
        }
    }
}
