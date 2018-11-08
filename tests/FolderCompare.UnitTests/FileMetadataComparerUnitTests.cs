﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolderCompare.UnitTests
{
    /// <summary>
    /// Summary description for UnitTest2
    /// </summary>
    [TestClass]
    public class FileMetadataComparerUnitTests
    {
        private FileMetadataComparer _sut = new FileMetadataComparer();
        private FileMetadata _lessThan;
        private FileMetadata _greaterThan;

        [TestInitialize()]
        public void TestInitialize()
        {
            _lessThan = new FileMetadata
            {
                Length = 100,
                LastWriteTimeUtc = DateTime.MinValue,
                RelativePathHash = "abcdefg",
            };

            _greaterThan = new FileMetadata
            {
                Length = 200,
                LastWriteTimeUtc = DateTime.MaxValue,
                RelativePathHash = "hijklmn",
            };
        }

        [TestMethod]
        public void Compare_SameInstance_AreEqual()
        {
            var cmp = _sut.Compare(_lessThan, _lessThan);

            Assert.AreEqual(FileMetadataComparer.Equal, cmp);
        }

        [TestMethod]
        public void Compare_SameValues_AreEqual()
        {
            var other = new FileMetadata
            {
                Length = _lessThan.Length,
                LastWriteTimeUtc = _lessThan.LastWriteTimeUtc,
                RelativePathHash = _lessThan.RelativePathHash,
            };

            var cmp = _sut.Compare(_lessThan, other);

            Assert.AreEqual(FileMetadataComparer.Equal, cmp);
        }

        #region null checks
        [TestMethod]
        public void Compare_BothNull_AreEqual()
        {
            var cmp = _sut.Compare(null, null);

            Assert.AreEqual(FileMetadataComparer.Equal, cmp);
        }

        [TestMethod]
        public void Compare_LeftNull_RightIsGreaterThanNull()
        {
            var cmp = _sut.Compare(null, _greaterThan);

            Assert.IsTrue(cmp < 0);
            Assert.AreEqual(FileMetadataComparer.RightOnly, cmp);
        }

        [TestMethod]
        public void Compare_RightNull_LeftIsGreaterThanNull()
        {
            var cmp = _sut.Compare(_lessThan, null);

            Assert.IsTrue(cmp > 0);
            Assert.AreEqual(FileMetadataComparer.LeftOnly, cmp);
        }
        #endregion

        #region Hash checks
        [TestMethod]
        public void Compare_RightHashIsGreater_RightHashGreater()
        {
            _greaterThan.LastWriteTimeUtc = _lessThan.LastWriteTimeUtc;
            _greaterThan.Length = _lessThan.Length;

            var cmp = _sut.Compare(_lessThan, _greaterThan);

            Assert.IsTrue(cmp < 0);
            Assert.AreEqual(FileMetadataComparer.RightRelPathGreater, cmp);
        }

        [TestMethod]
        public void Compare_LeftHashIsGreater_LeftHashGreater()
        {
            _greaterThan.LastWriteTimeUtc = _lessThan.LastWriteTimeUtc;
            _greaterThan.Length = _lessThan.Length;

            var cmp = _sut.Compare(_greaterThan, _lessThan);

            Assert.IsTrue(cmp > 0);
            Assert.AreEqual(FileMetadataComparer.LeftRelPathGreater, cmp);
        }
        #endregion

        #region Date checks
        [TestMethod]
        public void Compare_RightLastWriteTimeUtcIsMOreRecent_RightLastWriteGreater()
        {
            _greaterThan.RelativePathHash = _lessThan.RelativePathHash;
            _greaterThan.Length = _lessThan.Length;

            var cmp = _sut.Compare(_lessThan, _greaterThan);

            Assert.IsTrue(cmp < 0);
            Assert.AreEqual(FileMetadataComparer.RightLastWriteGreater, cmp);
        }

        [TestMethod]
        public void Compare_LeftLastWriteTimeUtcIsMOreRecent_LeftLastWriteGreater()
        {
            _greaterThan.RelativePathHash = _lessThan.RelativePathHash;
            _greaterThan.Length = _lessThan.Length;

            var cmp = _sut.Compare(_greaterThan, _lessThan);

            Assert.IsTrue(cmp > 0);
            Assert.AreEqual(FileMetadataComparer.LeftLastWriteGreater, cmp);
        }
        #endregion

        #region Length checks
        [TestMethod]
        public void Compare_RightFileLengthIsBigger_RightLengthGreater()
        {
            _greaterThan.RelativePathHash = _lessThan.RelativePathHash;
            _greaterThan.LastWriteTimeUtc = _lessThan.LastWriteTimeUtc;

            var cmp = _sut.Compare(_lessThan, _greaterThan);

            Assert.IsTrue(cmp < 0);
            Assert.AreEqual(FileMetadataComparer.RightLengthGreater, cmp);
        }

        [TestMethod]
        public void Compare_LeftFileLengthIsBigger_LeftLengthGreater()
        {
            _greaterThan.RelativePathHash = _lessThan.RelativePathHash;
            _greaterThan.LastWriteTimeUtc = _lessThan.LastWriteTimeUtc;

            var cmp = _sut.Compare(_greaterThan, _lessThan);

            Assert.IsTrue(cmp > 0);
            Assert.AreEqual(FileMetadataComparer.LeftLengthGreater, cmp);
        }
        #endregion
    }
}
