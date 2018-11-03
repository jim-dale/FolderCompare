using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolderCompare.UnitTests
{
    [TestClass]
    public class ComparisonReportUnitTests
    {
        private FileMetadata _item1;
        private FileMetadata _item2;

        [TestInitialize()]
        public void TestInitialize()
        {
            _item1 = new FileMetadata
            {
                RelativePath = @"abcdefg\hijklmnop\rstuvw\xyz\0123456789.dat",
                Length = 100,
                LastWriteTimeUtc = new DateTime(2016, 9, 8, 15, 26, 44),
            };

            _item2 = new FileMetadata
            {
                RelativePath = @"abcdefg\hijklmnop\rstuvw\xyz\0123456789.dat",
                Length = 200,
                LastWriteTimeUtc = DateTime.MaxValue,
            };
        }

        [TestMethod]
        public void GetAsTableRow_With_item1And_item2_CorrectlyFormattedTwoColumnString()
        {
            var expected = @"abcdefg\hijklmnop\rstuvw\xyz\0123456789.dat   2016-09-08 15:26:44Z   100 bytes | abcdefg\hijklmnop\rstuvw\xyz\0123456789.dat   9999-12-31 23:59:59Z   200 bytes";

            var sut = new ComparisonReport(DisplayMode.All, 160);
            string actual = sut.GetAsTableRow(_item1, _item2);

            Assert.AreEqual(expected, actual);
        }
    }
}
