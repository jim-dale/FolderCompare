
namespace FolderCompare
{
    using System;
    using System.IO;

    public static class FileSystemInfoExtensions
    {
        public static string GetRelativePathFrom(this FileSystemInfo to, FileSystemInfo from)
        {
            return from.GetRelativePathTo(to);
        }

        public static string GetRelativePathTo(this FileSystemInfo from, FileSystemInfo to)
        {
            var fromPath = GetPath(from);
            var toPath = GetPath(to);

            var fromUri = new Uri(fromPath);
            var toUri = new Uri(toPath);

            var relativeUri = fromUri.MakeRelativeUri(toUri);
            var relativePath = Uri.UnescapeDataString(relativeUri.OriginalString);

            return relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            string GetPath(FileSystemInfo fsi)
            {
                if (fsi is DirectoryInfo di)
                {
                    return di.FullName.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
                }
                return fsi.FullName;
            }
        }
    }
}
