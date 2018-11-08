
namespace FolderCompare
{
    using System;
    using System.Collections.Generic;

    public class FileMetadataComparer : Comparer<FileMetadata>
    {
        public const int Equal = 0;

        public const int LeftOnly = 1;
        public const int LeftRelPathGreater = 2;
        public const int LeftLastWriteGreater = 3;
        public const int LeftLengthGreater = 4;

        public const int RightOnly = -1;
        public const int RightRelPathGreater = -2;
        public const int RightLastWriteGreater = -3;
        public const int RightLengthGreater = -4;

        private readonly IComparer<string> _hashComparer = StringComparer.InvariantCultureIgnoreCase;
        private readonly IComparer<DateTime?> _dateComparer = Comparer<DateTime?>.Default;
        private readonly IComparer<long?> _lengthComparer = Comparer<long?>.Default;

        public override int Compare(FileMetadata left, FileMetadata right)
        {
            int result = Equal;

            if (left is null && right != null)
            {
                result = RightOnly;
            }
            else if (right is null && left != null)
            {
                result = LeftOnly;
            }
            else
            {
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
            }
            return result;
        }
    }
}
