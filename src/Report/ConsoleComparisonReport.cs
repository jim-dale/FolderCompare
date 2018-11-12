
namespace FolderCompare
{
    using System;
    using System.Text;

    public class ConsoleComparisonReport : IComparisonReport
    {
        private const int DateStrLength = 22;
        private const int LengthStrLength = 12;
        private const string DateHeader = "Modified";
        private const string LengthHeader = "Size";

        private readonly DisplayMode _displayMode;
        private readonly ContentsMode _contentsMode;
        private readonly int _deviceWidth;
        private readonly int _majorColumnWidth;
        private readonly int _pathMaxLen;

        private bool _showHeader = false;
        private string _leftSource;
        private string _rightSource;

        public ConsoleComparisonReport(DisplayMode displayMode, ContentsMode contentsMode, int deviceWidth)
        {
            _displayMode = displayMode;
            _contentsMode = contentsMode;
            _deviceWidth = deviceWidth;

            _majorColumnWidth = GetMaxColumnWidth(deviceWidth);
            _pathMaxLen = _majorColumnWidth - DateStrLength - LengthStrLength;
        }

        public void SetSources(string leftSource, string rightSource)
        {
            _leftSource = leftSource;
            _rightSource = rightSource;

            _showHeader = true;
        }

        public void OutputRow(CompareViewModel viewModel)
        {
            if (Helpers.GetShouldShowRow(_displayMode, _contentsMode, viewModel))
            {
                if (_showHeader)
                {
                    OutputHeader();
                    _showHeader = false;
                }
                //ConsoleColor colour = Console.ForegroundColor;
                //Console.ForegroundColor = colour;
                //var text = GetPathsAsTableRow(Console.WindowWidth, leftItem?.RelativePath, rightItem?.RelativePath);
                var text = GetAsRow(viewModel.LeftItem, viewModel.RightItem, viewModel.AreEqual);

                Console.WriteLine(text);
            }
            Console.ResetColor();
        }

        private void OutputHeader()
        {
            string p1 = NativeMethods.CompactPath(_leftSource, _pathMaxLen);
            string p2 = NativeMethods.CompactPath(_rightSource, _pathMaxLen);

            string str1 = GetAsColumn(p1, _pathMaxLen, DateHeader, DateStrLength, LengthHeader, LengthStrLength);
            string str2 = GetAsColumn(p2, _pathMaxLen, DateHeader, DateStrLength, LengthHeader, LengthStrLength);

            string row = JoinLeftAndRightColumns(str1, str2);

            Console.WriteLine(row);
            Console.WriteLine(new String('-', _deviceWidth - 1));
        }

        public string GetAsRow(FileMetadata item1, FileMetadata item2, bool? areEqual)
        {
            string part1 = GetAsColumn(item1);
            string part2 = GetAsColumn(item2);

            return JoinLeftAndRightColumns(part1, part2, areEqual);
        }

        public string GetAsColumn(FileMetadata item)
        {
            string result = String.Empty;

            if (item != null)
            {
                string dateStr = item.LastWriteTimeUtc.ToString("u");
                string lengthStr = NativeMethods.FormatByteSizeEx(item.Length, LengthStrLength);
                string path = NativeMethods.CompactPath(item.RelativePath, _pathMaxLen);

                result = GetAsColumn(path, _pathMaxLen, dateStr, DateStrLength, lengthStr, LengthStrLength);
            }

            return result;
        }

        private string GetAsColumn(string str1, int str1MaxLen, string str2, int str2MaxLen, string str3, int str3MaxLen)
        {
            StringBuilder sb = new StringBuilder(_majorColumnWidth);

            sb.AppendFormat(str1);
            sb.Append(' ', str1MaxLen - str1.Length);   // Pad left

            sb.Append(' ', str2MaxLen - str2.Length);   // Pad right
            sb.Append(str2);

            sb.Append(' ', str3MaxLen - str3.Length);   // Pad right
            sb.Append(str3);

            return sb.ToString();
        }

        private string JoinLeftAndRightColumns(string part1, string part2, bool? areEqual = null)
        {
            StringBuilder sb = new StringBuilder(_deviceWidth);

            sb.Append(part1);
            sb.Append(' ', _majorColumnWidth - part1.Length);
            sb.Append(GetEqualitySeparator(areEqual));
            sb.Append(part2);

            return sb.ToString();
        }

        private static string GetEqualitySeparator(bool? areEqual)
        {
            string result = " | ";

            switch (areEqual)
            {
                case false:
                    result = " \u2260 ";
                    break;
                case true:
                    result = " = ";
                    break;
                case null:
                default:
                    break;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        private static int GetMaxColumnWidth(int width)
        {
            return (width - 3) / 2;
        }
    }
}
