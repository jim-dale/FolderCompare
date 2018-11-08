
namespace FolderCompare
{
    public interface IComparisonReport
    {
        void SetSources(string leftSource, string rightSource);
        void OutputRow(FileMetadata leftItem, FileMetadata rightItem, int comparison, bool? areEqual = null);
    }
}
