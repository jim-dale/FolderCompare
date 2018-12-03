
namespace FolderCompare
{
    public static partial class Helpers
    {
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
                    result = (viewModel.RightItem is null || viewModel.LeftItem is null || viewModel.ContentsComparison != 0 || viewModel.RelPathComparison != 0 || viewModel.LastWriteComparison != 0 || viewModel.SizeComparison != 0);
                    break;
                case DisplayMode.Moved:
                    result = (viewModel.ContentsComparison == 0 && viewModel.RelPathComparison != 0);
                    break;
                case DisplayMode.Modified:
                    result = (viewModel.RightItem != null && viewModel.LeftItem != null && viewModel.LastWriteComparison != 0);
                    break;
                case DisplayMode.Size:
                    result = (viewModel.RightItem != null && viewModel.LeftItem != null && viewModel.SizeComparison != 0);
                    break;
                case DisplayMode.Corrupt:
                    result = (viewModel.ContentsComparison != 0 && viewModel.RelPathComparison == 0 && viewModel.LastWriteComparison == 0 && viewModel.SizeComparison == 0);
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
