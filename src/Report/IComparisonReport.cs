
namespace FolderCompare
{
    public interface IComparisonReport
    {
        void SetSources(string leftSource, string rightSource);
        void OutputRow(CompareViewModel viewModel);
    }
}
