
namespace FolderCompare.UnitTests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class JoinMetadataListsUnitTests
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
        public void TestMethod1()
        {
            var sut = new CompareCommand();

            //sut.GetItems(new List<FileMetadata> { _item1 }, new List<FileMetadata> { _item2 });
        }
    }
}
