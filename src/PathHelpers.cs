
namespace FolderCompare
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    public static class PathHelpers
    {
        [DllImport("shlwapi.dll")]
        extern static bool PathCompactPathEx([Out] StringBuilder pszOut, string szPath, int cchMax, int dwFlags);

        public static string TruncatePath(string path, int length)
        {
            string result = string.Empty;

            if (String.IsNullOrWhiteSpace(path) == false)
            {
                var sb = new StringBuilder(length + 1);
                PathCompactPathEx(sb, path, length + 1, 0);
                result = sb.ToString();
            }
            return result;
        }
    }
}
