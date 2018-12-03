
namespace FolderCompare
{
    using System;

    public static partial class Helpers
    {
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

        public static int GetComparisonResultAsExitCode(int cmp)
        {
            return (cmp == 0) ? ExitCode.FoldersAreTheSame : ExitCode.FoldersAreDifferent;
        }
    }
}
