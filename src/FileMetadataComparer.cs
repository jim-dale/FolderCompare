
namespace FolderCompare
{
    using System;
    using System.Collections.Generic;

    public class FileMetadataComparer : Comparer<FileMetadata>
    {
        private readonly IComparer<FileMetadata> _hashComparer = new HashComparer();
        private IComparer<DateTime?> _dateComparer=Comparer<Nullable<DateTime>>.Default;
        private IComparer<long?> _lengthComparer = Comparer<Nullable<long>>.Default;

        public const int Equal = 0;
        public const int LeftHashGreater = 1;
        public const int LeftLastWriteGreater = 2;
        public const int LeftLengthGreater = 3;
        public const int RightHashGreater = 1;
        public const int RightLastWriteGreater = -2;
        public const int RightLengthGreater = -3;

        public override int Compare(FileMetadata left, FileMetadata right)
        {
            int result = Equal;

            int cmp = string.Compare(left?.Hash, right?.Hash);
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
                result = (cmp > 0) ? LeftHashGreater : RightHashGreater;
            }

            return result;
        }
    }
}
