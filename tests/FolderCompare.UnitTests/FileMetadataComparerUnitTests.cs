using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolderCompare.UnitTests
{
    /// <summary>
    /// Summary description for UnitTest2
    /// </summary>
    [TestClass]
    public class FileMetadataComparerUnitTests
    {
        private FileMetadata _lessThan;
        private FileMetadata _greaterThan;

        [TestInitialize()]
        public void TestInitialize()
        {
            _lessThan = new FileMetadata
            {
                Length = 100,
                LastWriteTimeUtc = DateTime.MinValue,
                RelativePath = "abcdefg",
            };

            _greaterThan = new FileMetadata
            {
                Length = 200,
                LastWriteTimeUtc = DateTime.MaxValue,
                RelativePath = "hijklmn",
            };
        }

        [TestMethod]
        public void Compare_SameInstance_AreEqual()
        {
            var vm = Helpers.CreateViewModel(_lessThan, _lessThan);

            Assert.AreEqual(0, vm.RelPathComparison);
            Assert.AreEqual(0, vm.LastWriteComparison);
            Assert.AreEqual(0, vm.SizeComparison);
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

            var vm = Helpers.CreateViewModel(_lessThan, _lessThan);

            Assert.AreEqual(0, vm.RelPathComparison);
            Assert.AreEqual(0, vm.LastWriteComparison);
            Assert.AreEqual(0, vm.SizeComparison);
        }

        #region null checks
        [TestMethod]
        public void Compare_BothNull_AreEqual()
        {
            var vm = Helpers.CreateViewModel(_lessThan, _lessThan);

            Assert.AreEqual(0, vm.RelPathComparison);
            Assert.AreEqual(0, vm.LastWriteComparison);
            Assert.AreEqual(0, vm.SizeComparison);
        }

        [TestMethod]
        public void Compare_LeftNull_RightIsGreaterThanNull()
        {
            var vm = Helpers.CreateViewModel(null, _greaterThan);

            Assert.IsTrue(vm.RelPathComparison < 0);
            Assert.IsTrue(vm.LastWriteComparison < 0);
            Assert.IsTrue(vm.SizeComparison < 0);
        }

        [TestMethod]
        public void Compare_RightNull_LeftIsGreaterThanNull()
        {
            var vm = Helpers.CreateViewModel(_lessThan, null);

            Assert.IsTrue(vm.RelPathComparison > 0);
            Assert.IsTrue(vm.LastWriteComparison > 0);
            Assert.IsTrue(vm.SizeComparison > 0);
        }
        #endregion

        #region Hash checks
        [TestMethod]
        public void Compare_RightHashIsGreater_RightHashGreater()
        {
            _greaterThan.LastWriteTimeUtc = _lessThan.LastWriteTimeUtc;
            _greaterThan.Length = _lessThan.Length;

            var vm = Helpers.CreateViewModel(_lessThan, _greaterThan);

            Assert.IsTrue(vm.RelPathComparison < 0);
            Assert.AreEqual(0, vm.LastWriteComparison);
            Assert.AreEqual(0, vm.SizeComparison);
        }

        [TestMethod]
        public void Compare_LeftHashIsGreater_LeftHashGreater()
        {
            _greaterThan.LastWriteTimeUtc = _lessThan.LastWriteTimeUtc;
            _greaterThan.Length = _lessThan.Length;

            var vm = Helpers.CreateViewModel(_greaterThan, _lessThan);

            Assert.IsTrue(vm.RelPathComparison > 0);
            Assert.AreEqual(0, vm.LastWriteComparison);
            Assert.AreEqual(0, vm.SizeComparison);
        }
        #endregion

        #region Date checks
        [TestMethod]
        public void Compare_RightLastWriteTimeUtcIsMOreRecent_RightLastWriteGreater()
        {
            _greaterThan.RelativePath = _lessThan.RelativePath;
            _greaterThan.Length = _lessThan.Length;

            var vm = Helpers.CreateViewModel(_lessThan, _greaterThan);

            Assert.IsTrue(vm.LastWriteComparison < 0);
            Assert.AreEqual(0, vm.RelPathComparison);
            Assert.AreEqual(0, vm.SizeComparison);
        }

        [TestMethod]
        public void Compare_LeftLastWriteTimeUtcIsMOreRecent_LeftLastWriteGreater()
        {
            _greaterThan.RelativePath = _lessThan.RelativePath;
            _greaterThan.Length = _lessThan.Length;

            var vm = Helpers.CreateViewModel(_greaterThan, _lessThan);

            Assert.IsTrue(vm.LastWriteComparison > 0);
            Assert.AreEqual(0, vm.RelPathComparison);
            Assert.AreEqual(0, vm.SizeComparison);
        }
        #endregion

        #region Length checks
        [TestMethod]
        public void Compare_RightFileLengthIsBigger_RightLengthGreater()
        {
            _greaterThan.RelativePath = _lessThan.RelativePath;
            _greaterThan.LastWriteTimeUtc = _lessThan.LastWriteTimeUtc;

            var vm = Helpers.CreateViewModel(_lessThan, _greaterThan);

            Assert.IsTrue(vm.SizeComparison < 0);
            Assert.AreEqual(0, vm.RelPathComparison);
            Assert.AreEqual(0, vm.LastWriteComparison);
        }

        [TestMethod]
        public void Compare_LeftFileLengthIsBigger_LeftLengthGreater()
        {
            _greaterThan.RelativePath = _lessThan.RelativePath;
            _greaterThan.LastWriteTimeUtc = _lessThan.LastWriteTimeUtc;

            var vm = Helpers.CreateViewModel(_greaterThan, _lessThan);

            Assert.IsTrue(vm.SizeComparison > 0);
            Assert.AreEqual(0, vm.RelPathComparison);
            Assert.AreEqual(0, vm.LastWriteComparison);
        }
        #endregion
    }
}
