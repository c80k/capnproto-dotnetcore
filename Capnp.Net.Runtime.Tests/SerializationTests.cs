using Capnp.Net.Runtime.Tests.GenImpls;
using Capnproto_test.Capnp.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capnp.Net.Runtime.Tests
{
    [TestClass]
    [TestCategory("Coverage")]
    public class SerializationTests
    {
        [TestMethod]
        public void ListOfBits()
        {
            void CheckList(IEnumerable<bool> items)
            {
                int i = 0;
                foreach (bool bit in items)
                {
                    if (i == 63 || i == 66 || i == 129)
                        Assert.IsTrue(bit);
                    else
                        Assert.IsFalse(bit);

                    ++i;
                }
                Assert.AreEqual(130, i);
            }

            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfBitsSerializer>();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => list.Init(-1));
            // Assert.AreEqual(0, list.Count); // Bug or feature? Uninitialized list's Count is -1
            Assert.ThrowsException<InvalidOperationException>(() => { var _ = list[0]; });
            Assert.ThrowsException<InvalidOperationException>(() => { list[0] = false; });
            list.Init(130);
            list[63] = true;
            list[65] = true;
            list[66] = true;
            list[65] = false;
            list[129] = true;
            Assert.ThrowsException<IndexOutOfRangeException>(() => { var _ = list[130]; });
            Assert.ThrowsException<IndexOutOfRangeException>(() => { list[130] = false; });
            Assert.IsFalse(list[0]);
            Assert.IsTrue(list[63]);
            Assert.IsFalse(list[64]);
            Assert.IsFalse(list[65]);
            Assert.IsTrue(list[66]);
            Assert.IsTrue(list[129]);
            var list2 = b.CreateObject<ListOfBitsSerializer>();
            list2.Init(null);
            list2.Init(list);
            Assert.IsFalse(list2[0]);
            Assert.IsTrue(list2[63]);
            Assert.IsFalse(list2[64]);
            Assert.IsFalse(list2[65]);
            Assert.IsTrue(list2[66]);
            Assert.IsTrue(list2[129]);
            CheckList(list2);
            Assert.ThrowsException<InvalidOperationException>(() => list.Init(4));
            DeserializerState d = list2;
            var list3 = d.RequireList().CastBool();
            CheckList(list3);
        }

        [TestMethod]
        public void ListOfCaps()
        {
            var b = MessageBuilder.Create();
            b.InitCapTable();
            var list = b.CreateObject<ListOfCapsSerializer<ITestInterface>>();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => list.Init(-1));
            Assert.ThrowsException<InvalidOperationException>(() => { var _ = list[0]; });
            Assert.ThrowsException<InvalidOperationException>(() => { list[0] = null; });
            list.Init(5);
            Assert.ThrowsException<InvalidOperationException>(() => list.Init(1));
            var c1 = new Counters();
            var cap1 = new TestInterfaceImpl(c1);
            var c2 = new Counters();
            var cap2 = new TestInterfaceImpl(c2);
            list[0] = null;
            list[1] = cap1;
            list[2] = cap2;
            list[3] = cap1;
            list[4] = cap2;
            list[3] = null;
            Assert.IsTrue(list.All(p => p is Rpc.Proxy));
            var proxies = list.Cast<Rpc.Proxy>().ToArray();
            Assert.IsTrue(proxies[0].IsNull);
            Assert.IsFalse(proxies[1].IsNull);
            Assert.IsTrue(proxies[3].IsNull);
            list[2].Foo(123u, true).Wait();
            Assert.AreEqual(0, c1.CallCount);
            Assert.AreEqual(1, c2.CallCount);
            list[4].Foo(123u, true).Wait();
            Assert.AreEqual(2, c2.CallCount);

            var list2 = b.CreateObject<ListOfCapsSerializer<ITestInterface>>();
            list2.Init(null);
            list2.Init(list);
            proxies = list2.Cast<Rpc.Proxy>().ToArray();
            Assert.IsTrue(proxies[0].IsNull);
            Assert.IsFalse(proxies[1].IsNull);
            Assert.IsTrue(proxies[3].IsNull);
            list2[2].Foo(123u, true).Wait();
            Assert.AreEqual(0, c1.CallCount);
            Assert.AreEqual(3, c2.CallCount);
            list2[4].Foo(123u, true).Wait();
            Assert.AreEqual(4, c2.CallCount);

            DeserializerState d = list2;
            var list3 = d.RequireList().CastCapList<ITestInterface>();
            proxies = list3.Cast<Rpc.Proxy>().ToArray();
            Assert.IsTrue(proxies[0].IsNull);
            Assert.IsFalse(proxies[1].IsNull);
            Assert.IsTrue(proxies[3].IsNull);
            list3[2].Foo(123u, true).Wait();
            Assert.AreEqual(0, c1.CallCount);
            Assert.AreEqual(5, c2.CallCount);
            list3[4].Foo(123u, true).Wait();
            Assert.AreEqual(6, c2.CallCount);
        }
    }
}
