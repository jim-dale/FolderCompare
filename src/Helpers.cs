
namespace FolderCompare
{
    using System;
    using System.IO;

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
                RelativePathHash = HashStream.GetStringHashSHA512(rp.ToLowerInvariant()),
            };

            return result;
        }

        public static int GetComparisonResultAsExitCode(int cmp)
        {
            return (cmp == 0) ? ExitCode.FoldersAreTheSame : ExitCode.FoldersAreDifferent;
        }
    }
}
