
namespace FolderCompare
{
    public interface IComparisonReport
    {
        void OutputHeader(string leftSource, string rightSource);
        void OutputRow(FileMetadata item1, FileMetadata item2, int comparison);
    }
}
