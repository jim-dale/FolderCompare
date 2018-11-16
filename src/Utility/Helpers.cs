
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

        public static string EnumNamesAsString<T>(T defaultValue)
        {
            Type type = typeof(T);
            string result = String.Join(", ", Enum.GetNames(type));
            result += ". The default is " + Enum.GetName(type, defaultValue);
            return result;
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

        public static CompareViewModel CreateViewModel(FileMetadata leftItem, FileMetadata rightItem, IComparer<FileMetadata> comparer, IEqualityComparer<FileMetadata> equalityComparer)
        {
            int comparison = comparer.Compare(leftItem, rightItem);
            bool? areEqual = (leftItem is null || rightItem is null) ? (bool?)null : equalityComparer.Equals(leftItem, rightItem);

            return new CompareViewModel
            {
                LeftItem = leftItem,
                RightItem = rightItem,
                Comparison = comparison,
                AreEqual = areEqual
            };
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

        public static void GenerateContentsHash(IEnumerable<FileMetadata> items, bool force)
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

        public static bool GetShouldShowRow(DisplayMode displayMode, ContentsMode contentsMode, CompareViewModel viewModel)
        {
            var result = GetShouldShowRow(displayMode, viewModel.LeftItem, viewModel.RightItem, viewModel.Comparison);
            if (result)
            {
                result = GetShouldShowRow(contentsMode, viewModel.LeftItem, viewModel.RightItem, viewModel.AreEqual);
            }
            return result;
        }

        public static int GetComparisonResultAsExitCode(int cmp)
        {
            return (cmp == 0) ? ExitCode.FoldersAreTheSame : ExitCode.FoldersAreDifferent;
        }

        private static bool GetShouldShowRow(DisplayMode mode, FileMetadata leftItem, FileMetadata rightItem, int comparison)
        {
            bool result = false;

            switch (mode)
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

        private static bool GetShouldShowRow(ContentsMode mode, FileMetadata leftItem, FileMetadata rightItem, bool? areEqual)
        {
            bool result = false;

            switch (mode)
            {
                case ContentsMode.Same:
                    result = (areEqual.HasValue && areEqual.Value == true);
                    break;
                case ContentsMode.Differences:
                    result = (areEqual.HasValue && areEqual.Value == false);
                    break;
                case ContentsMode.All:
                    result = true;
                    break;
                case ContentsMode.None:
                default:
                    break;
            }

            return result;
        }
    }
}
