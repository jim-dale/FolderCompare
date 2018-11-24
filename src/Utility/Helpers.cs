
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

        public static CompareViewModel CreateViewModel(FileMetadata leftItem, FileMetadata rightItem)
        {
            IComparer<string> hashComparer = StringComparer.InvariantCultureIgnoreCase;
            IComparer<DateTime?> dateComparer = Comparer<DateTime?>.Default;
            IComparer<long?> sizeComparer = Comparer<long?>.Default;

            return new CompareViewModel
            {
                LeftItem = leftItem,
                RightItem = rightItem,
                RelPathComparison = hashComparer.Compare(leftItem?.RelativePath, rightItem?.RelativePath),
                LastWriteComparison = dateComparer.Compare(leftItem?.LastWriteTimeUtc, rightItem?.LastWriteTimeUtc),
                SizeComparison = sizeComparer.Compare(leftItem?.Length, rightItem?.Length),
                ContentsComparison = hashComparer.Compare(leftItem?.ContentsHash, rightItem?.ContentsHash),
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
                RelativePathHash = HashHelpers.GetStringHashSHA512(rp.ToLowerInvariant()),
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
                item.ContentsHash = HashHelpers.GetFileHashSHA512(item.OriginalPath);
            }
        }

        public static int GetComparisonResultAsExitCode(int cmp)
        {
            return (cmp == 0) ? ExitCode.FoldersAreTheSame : ExitCode.FoldersAreDifferent;
        }

        public static bool GetShouldShowRow(DisplayMode mode, CompareViewModel viewModel)
        {
            bool result = false;

            switch (mode)
            {
                case DisplayMode.Same:
                    result = (viewModel.ContentsComparison == 0 && viewModel.RelPathComparison == 0 && viewModel.LastWriteComparison == 0 && viewModel.SizeComparison == 0);
                    break;
                case DisplayMode.LeftOnly:
                    result = (viewModel.RightItem is null);
                    break;
                case DisplayMode.RightOnly:
                    result = (viewModel.LeftItem is null);
                    break;
                case DisplayMode.Different:
                    result = (viewModel.ContentsComparison != 0 || viewModel.RelPathComparison != 0 || viewModel.LastWriteComparison != 0 || viewModel.SizeComparison != 0);
                    break;
                case DisplayMode.Moved:
                    result = (viewModel.ContentsComparison == 0 && viewModel.RelPathComparison != 0);
                    break;
                case DisplayMode.Modified:
                    result = (viewModel.LastWriteComparison != 0);
                    break;
                case DisplayMode.Size:
                    result = (viewModel.SizeComparison != 0);
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
    }
}
