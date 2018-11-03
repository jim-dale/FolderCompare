
namespace FolderCompare
{
    using System;
    using System.Collections.Generic;

    public class FileMetadataComparer : Comparer<FileMetadata>
    {
        public const int Equal = 0;
        public const int LeftRelPathGreater = 1;
        public const int LeftLastWriteGreater = 2;
        public const int LeftLengthGreater = 3;
        public const int RightRelPathGreater = 1;
        public const int RightLastWriteGreater = -2;
        public const int RightLengthGreater = -3;

        private readonly IComparer<String> _hashComparer = Comparer<String>.Default;
        private readonly IComparer<DateTime?> _dateComparer = Comparer<DateTime?>.Default;
        private readonly IComparer<long?> _lengthComparer = Comparer<long?>.Default;

        public override int Compare(FileMetadata left, FileMetadata right)
        {
            int result = Equal;

            int cmp = _hashComparer.Compare(left?.RelativePathHash, right?.RelativePathHash);
            if (cmp == Equal)
            {
                cmp = _dateComparer.Compare(left?.LastWriteTimeUtc, right?.LastWriteTimeUtc);
                if (cmp == Equal)
                {
                    cmp = _lengthComparer.Compare(left?.Length, right?.Length);
                    if (cmp != Equal)
                    {
                        result = (cmp > 0) ? LeftLengthGreater : RightLengthGreater;
                    }
                }
                else
                {
                    result = (cmp > 0) ? LeftLastWriteGreater : RightLastWriteGreater;
                }
            }
            else
            {
                result = (cmp > 0) ? LeftRelPathGreater : RightRelPathGreater;
            }

            return result;
        }
    }
}
