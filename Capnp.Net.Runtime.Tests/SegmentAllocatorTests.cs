using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capnp.Net.Runtime.Tests
{
    [TestClass]
    public class SegmentAllocatorTests
    {
        [TestMethod]
        public void BasicSegmentAllocator()
        {
            var alloc = new SegmentAllocator(128);

            Assert.IsTrue(alloc.Allocate(1, 0, out var slice1, false));
            Assert.AreEqual(0u, slice1.SegmentIndex);
            Assert.AreEqual(0, slice1.Offset);

            Assert.IsTrue(alloc.Allocate(1, 1, out var slice2, false));
            Assert.AreEqual(0u, slice2.SegmentIndex);
            Assert.AreEqual(1, slice2.Offset);

            Assert.IsTrue(alloc.Allocate(127, 0, out var slice3, false));
            Assert.AreEqual(1u, slice3.SegmentIndex);
            Assert.AreEqual(0, slice3.Offset);

            Assert.IsFalse(alloc.Allocate(127, 0, out var slice4, true));
            Assert.IsFalse(alloc.Allocate(127, 1, out var slice5, true));

            Assert.IsTrue(alloc.Allocate(2, 0, out var slice6, true));
            Assert.AreEqual(0u, slice6.SegmentIndex);
            Assert.AreEqual(2, slice6.Offset);

            Assert.IsTrue(alloc.Allocate(1, 1, out var slice7, true));
            Assert.AreEqual(1u, slice7.SegmentIndex);
            Assert.AreEqual(127, slice7.Offset);

            Assert.IsTrue(alloc.Allocate(129, 0, out var slice8, false));
            Assert.AreEqual(2u, slice8.SegmentIndex);
            Assert.AreEqual(0, slice8.Offset);
        }
    }
}
