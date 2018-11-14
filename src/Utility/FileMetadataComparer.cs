
namespace FolderCompare
{
    using System;
    using System.Collections.Generic;

    public class FileMetadataComparer : Comparer<FileMetadata>
    {
        #region Comparison result constants
        public const int Equal = 0;
        public const int LeftOnly = 1;
        public const int LeftRelPathGreater = 2;
        public const int LeftLastWriteGreater = 3;
        public const int LeftLengthGreater = 4;
        public const int RightOnly = -1;
        public const int RightRelPathGreater = -2;
        public const int RightLastWriteGreater = -3;
        public const int RightLengthGreater = -4;
        #endregion

        #region Private fields
        private readonly IComparer<string> _hashComparer = StringComparer.InvariantCultureIgnoreCase;
        private readonly IComparer<DateTime?> _dateComparer = Comparer<DateTime?>.Default;
        private readonly IComparer<long?> _lengthComparer = Comparer<long?>.Default;
        #endregion

        /// <summary>
        /// Compare two FileMetadata objects
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override int Compare(FileMetadata x, FileMetadata y)
        {
            int result = Equal;

            if (x is null && y != null)
            {
                result = RightOnly;
            }
            else if (y is null && x != null)
            {
                result = LeftOnly;
            }
            else
            {
                int cmp = _hashComparer.Compare(x?.RelativePathHash, y?.RelativePathHash);
                if (cmp == Equal)
                {
                    cmp = _dateComparer.Compare(x?.LastWriteTimeUtc, y?.LastWriteTimeUtc);
                    if (cmp == Equal)
                    {
                        cmp = _lengthComparer.Compare(x?.Length, y?.Length);
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
