
namespace FolderCompare
{
    public interface IComparisonReport
    {
        void OutputHeader(string leftSource, string rightSource);
        void OutputRow(FileMetadata leftItem, FileMetadata rightItem, int comparison);
    }
}
