
namespace FolderCompare
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    public static partial class NativeMethods
    {
        [Flags]
        public enum SFBS_FLAGS
        {
            /// <summary>Round to the nearest displayed digit.</summary>
            ROUND_TO_NEAREST_DISPLAYED_DIGIT = 1,

            /// <summary>Discard undisplayed digits.</summary>
            TRUNCATE_UNDISPLAYED_DECIMAL_DIGITS = 2,
        }

        [DllImport("shlwapi.dll", SetLastError = false, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private extern static bool PathCompactPathEx(StringBuilder pszOut, string pszSrc, uint cchMax, uint dwFlags = 0);

        public static string CompactPath(string path, int maxLength)
        {
            string result = string.Empty;

            if (String.IsNullOrWhiteSpace(path) == false)
            {
                int bufferSize = maxLength + 1;
                var sb = new StringBuilder(bufferSize, bufferSize);
                PathCompactPathEx(sb, path, (uint)bufferSize);
                result = sb.ToString();
            }
            return result;
        }

        [DllImport("shlwapi.dll", SetLastError = false, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private extern static Int32 StrFormatByteSizeEx(ulong ull, SFBS_FLAGS flags, StringBuilder pszBuf, uint cchBuf);

        public static string FormatByteSizeEx(long fileLength, int maxStringLength)
        {
            int bufferSize = maxStringLength + 1;
            var sb = new StringBuilder(bufferSize, bufferSize);
            StrFormatByteSizeEx((ulong)fileLength, SFBS_FLAGS.ROUND_TO_NEAREST_DISPLAYED_DIGIT, sb, (uint)bufferSize);
            return sb.ToString();
        }

        [DllImport("shlwapi.dll", SetLastError = false, EntryPoint = "StrFormatByteSizeW", CharSet = CharSet.Unicode)]
        private extern static Int32 StrFormatByteSize(long qdw, StringBuilder pszBuf, uint cchBuf);
        /// <summary>
        /// Converts a numeric value into a string that represents the number
        /// expressed as a size value in bytes, kilobytes, megabytes, or gigabytes, depending on the size.
        /// </summary>
        /// <param name="fileLength">The numeric value to be converted.</param>
        /// <returns>the converted string</returns>
        public static string FormatByteSize(long fileLength, int maxStringLength)
        {
            int bufferSize = maxStringLength + 1;
            var sb = new StringBuilder(bufferSize, bufferSize);
            StrFormatByteSize(fileLength, sb, (uint)bufferSize);
            return sb.ToString();
        }
    }
}
