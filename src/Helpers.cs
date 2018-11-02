
namespace FolderCompare
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public static class Helpers
    {
        public const string JsonExtension = ".json";

        public static string ExpandPath(string path)
        {
            return Environment.ExpandEnvironmentVariables(path);
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
                PathHash = HashStream.GetStringHashSHA512(rp.ToLowerInvariant()),
            };

            return result;
        }

        public static IEnumerable<ValueTuple<FileMetadata, FileMetadata>> GetFullOuterJoin(IEnumerable<FileMetadata> items1, IEnumerable<FileMetadata> items2, IEqualityComparer<FileMetadata> comparer)
        {
            var items = items1.Union(items2, comparer);

            foreach (var item in items)
            {
                var item1 = items1.SingleOrDefault(i => i.Hash == item.Hash);
                var item2 = items2.SingleOrDefault(i => i.Hash == item.Hash);
                yield return (item1, item2);
            }
        }

        public static void ShowDifferenceResult(FileMetadata leftItem, FileMetadata rightItem, DisplayMode outputType, int comparison)
        {
            bool show = false;

            switch (outputType)
            {
                case DisplayMode.LeftOnly:
                    show = rightItem is null;
                    break;
                case DisplayMode.RightOnly:
                    show = leftItem is null;
                    break;
                case DisplayMode.Differences:
                    show = leftItem is null || rightItem is null;
                    break;
                case DisplayMode.All:
                    show = true;
                    break;
                case DisplayMode.None:
                default:
                    break;
            }
            if (show)
            {
                //ConsoleColor colour = Console.ForegroundColor;
                //Console.ForegroundColor = colour;
                var text = GetPathsAsTableRow(Console.WindowWidth, leftItem?.RelativePath, rightItem?.RelativePath);
                Console.WriteLine(text);
            }
            Console.ResetColor();
        }

        public static string GetPathsAsTableRow(int width, string path1, string path2)
        {
            int maxLength = (width - 3) / 2;

            string s1 = PathHelpers.TruncatePath(path1, maxLength);
            string s2 = PathHelpers.TruncatePath(path2, maxLength);

            StringBuilder sb = new StringBuilder(width);

            sb.Append(s1);
            sb.Append(' ', maxLength - s1.Length);
            sb.Append(" | ");
            sb.Append(s2);

            return sb.ToString();
        }

        public static int GetComparisionResultAsExitCode(int cmp)
        {
            return (cmp == 0) ? ExitCode.FoldersAreTheSame : ExitCode.FoldersAreDifferent;
        }
    }
}
