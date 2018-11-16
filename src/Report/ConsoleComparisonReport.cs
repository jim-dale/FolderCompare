﻿
namespace FolderCompare
{
    using System;
    using System.Text;

    public partial class ConsoleComparisonReport : IComparisonReport
    {
        private const int DateStrLength = 20;
        private const int SizeStrLength = 10;
        private const int ColumnSeparatorWidth = 3;
        private const int NumberOfFields = 3;
        private const string DateHeader = "Modified";
        private const string SizeHeader = "Size";
        private const char HorizontalLineChar = '\u2500';
        private const char VerticalLineChar = '\u2502';
        private const char DownHorizontalChar = '\u252C';
        private const char VerticalHorizontalChar = '\u253C';
        private const char DoubleVerticalChar = '\u2551';
        private const char DoubleVertSingleHoriz = '\u256B';

        private const char PaddingChar = ' ';
        private const char EqualChar = '=';
        private const char NotEqualChar = '\u2260';

        private readonly DisplayMode _displayMode;
        private readonly ContentsMode _contentsMode;

        private string _leftSource;
        private string _rightSource;
        private int _deviceWidth;
        private bool _showHeader = false;
        private int _majorColumnWidth;
        private int _pathColumnWidth;
        private int _dateColumnWidth;
        private int _sizeColumnWidth;

        private int _pathMaxLength;
        private int _dateStrMaxLength = 20;
        private int _sizeStrMaxLength = 10;

        private class RowViewModel
        {
            public ConsoleColor Colour { get; set; }
            public string Text { get; set; }
        }

        public ConsoleComparisonReport(DisplayMode displayMode, ContentsMode contentsMode)
        {
            _displayMode = displayMode;
            _contentsMode = contentsMode;

            _showHeader = true;
        }

        public void SetSources(string leftSource, string rightSource)
        {
            _leftSource = leftSource;
            _rightSource = rightSource;
        }

        public void OutputRow(CompareViewModel viewModel)
        {
            CalculateWidths();

            if (Helpers.GetShouldShowRow(_displayMode, _contentsMode, viewModel))
            {
                if (_showHeader)
                {
                    OutputHeader();
                    _showHeader = false;
                }

                Console.Write(GetPathAsJustifiedString(viewModel.LeftItem?.RelativePath, Justification.Left));
                Console.Write(VerticalLineChar);
                Console.Write(GetDateAsJustifiedString(viewModel.LeftItem?.LastWriteTimeUtc, Justification.Left));
                Console.Write(VerticalLineChar);
                Console.Write(GetSizeAsJustifiedString(viewModel.LeftItem?.Length, Justification.Right));

                Console.Write(GetEqualitySeparator(viewModel.AreEqual));

                Console.Write(GetPathAsJustifiedString(viewModel.RightItem?.RelativePath, Justification.Left));
                Console.Write(VerticalLineChar);
                Console.Write(GetDateAsJustifiedString(viewModel.RightItem?.LastWriteTimeUtc, Justification.Left));
                Console.Write(VerticalLineChar);
                Console.Write(GetSizeAsJustifiedString(viewModel.RightItem?.Length, Justification.Right));

                Console.WriteLine();
            }
            Console.ResetColor();
        }

        private void OutputHeader()
        {
            CalculateWidths();

            Console.Write(GetPathAsJustifiedString(_leftSource, Justification.Left));
            Console.Write(VerticalLineChar);
            Console.Write(JustifyString(DateHeader, _dateColumnWidth, Justification.Left));
            Console.Write(VerticalLineChar);
            Console.Write(JustifyString(SizeHeader, _sizeColumnWidth, Justification.Right));

            Console.Write(GetEqualitySeparator(null));

            Console.Write(GetPathAsJustifiedString(_rightSource, Justification.Left));
            Console.Write(VerticalLineChar);
            Console.Write(JustifyString(DateHeader, _dateColumnWidth, Justification.Left));
            Console.Write(VerticalLineChar);
            Console.Write(JustifyString(SizeHeader, _sizeColumnWidth, Justification.Right));

            Console.WriteLine();

            DrawHorizontalSeparator();
        }

        private void DrawHorizontalSeparator()
        {
            RepeatCharacter(HorizontalLineChar, _pathColumnWidth);
            Console.Write(VerticalHorizontalChar);
            RepeatCharacter(HorizontalLineChar, _dateColumnWidth);
            Console.Write(VerticalHorizontalChar);
            RepeatCharacter(HorizontalLineChar, _sizeColumnWidth);

            Console.Write(HorizontalLineChar);
            Console.Write(DoubleVertSingleHoriz);
            Console.Write(HorizontalLineChar);

            RepeatCharacter(HorizontalLineChar, _pathColumnWidth);
            Console.Write(VerticalHorizontalChar);
            RepeatCharacter(HorizontalLineChar, _dateColumnWidth);
            Console.Write(VerticalHorizontalChar);
            RepeatCharacter(HorizontalLineChar, _sizeColumnWidth);
            Console.WriteLine();
        }

        private string GetPathAsJustifiedString(string value, Justification justification)
        {
            string result = String.Empty;
            if (String.IsNullOrEmpty(value) == false)
            {
                result = NativeMethods.CompactPath(value, _pathMaxLength);
            }
            result = JustifyString(result, _pathColumnWidth, justification);

            return result;
        }

        private string GetDateAsJustifiedString(DateTime? value, Justification justification)
        {
            string result = String.Empty;
            if (value.HasValue)
            {
                result = value.Value.ToString("u");
            }
            result = JustifyString(result, _dateColumnWidth, justification);

            return result;
        }

        private string GetSizeAsJustifiedString(long? value, Justification justification)
        {
            string result = String.Empty;
            if (value.HasValue)
            {
                result = NativeMethods.FormatByteSizeEx(value.Value, _sizeStrMaxLength);
            }
            result = JustifyString(result, _sizeColumnWidth, justification);

            return result;
        }

        private string JustifyString(string str, int maxWidth, Justification justification)
        {
            StringBuilder sb = new StringBuilder(maxWidth);

            int remaining = maxWidth - str.Length;
            int leftPadding = 0;
            int rightPadding = 0;

            if (remaining > 0)
            {
                if (justification == Justification.Right)
                {
                    leftPadding = remaining;
                }
                else if (justification == Justification.Left)
                {
                    rightPadding = remaining;
                }
                else
                {
                    leftPadding = (remaining + 1) / 2;
                    rightPadding = remaining - leftPadding;
                }
            }
            sb.Append(PaddingChar, leftPadding);
            sb.Append(str);
            sb.Append(PaddingChar, rightPadding);

            return sb.ToString();
        }

        private static string GetEqualitySeparator(bool? areEqual)
        {
            string result = " " + DoubleVerticalChar + " ";

            switch (areEqual)
            {
                case false:
                    result = " " + NotEqualChar + " ";
                    break;
                case true:
                    result = " " + EqualChar + " ";
                    break;
                case null:
                default:
                    break;
            }
            return result;
        }

        private void CalculateWidths()
        {
            _deviceWidth = Console.BufferWidth;
            _majorColumnWidth = GetMajorColumnWidth(_deviceWidth);
            _dateStrMaxLength = DateStrLength;
            _sizeStrMaxLength = SizeStrLength;
            _dateColumnWidth = _dateStrMaxLength;
            _sizeColumnWidth = _sizeStrMaxLength + 1;
            _pathColumnWidth = _majorColumnWidth - _dateColumnWidth - _sizeColumnWidth - (NumberOfFields - 1);
            _pathMaxLength = _pathColumnWidth - 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        private static int GetMajorColumnWidth(int width)
        {
            return (width - ColumnSeparatorWidth) / 2;
        }

        private static void RepeatCharacter(char ch, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Console.Write(ch);
            }
        }
    }
}
