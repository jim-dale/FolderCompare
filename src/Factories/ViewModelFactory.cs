
namespace FolderCompare
{
    using System;
    using System.Collections.Generic;

    public static class ViewModelFactory
    {
        public static CompareViewModel Create(FileMetadata leftItem, FileMetadata rightItem)
        {
            IComparer<string> hashComparer = StringComparer.InvariantCultureIgnoreCase;
            IComparer<DateTime?> dateComparer = Comparer<DateTime?>.Default;
            IComparer<long?> sizeComparer = Comparer<long?>.Default;

            return new CompareViewModel
            {
                LeftItem = leftItem,
                RightItem = rightItem,
                RelPathComparison = hashComparer.Compare(leftItem?.RelativePath, rightItem?.RelativePath),
                LastWriteComparison = dateComparer.Compare(leftItem?.LastWriteTimeUtc, rightItem?.LastWriteTimeUtc),
                SizeComparison = sizeComparer.Compare(leftItem?.Length, rightItem?.Length),
                ContentsComparison = hashComparer.Compare(leftItem?.ContentsHash, rightItem?.ContentsHash),
            };
        }
    }
}
