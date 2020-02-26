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
            list.Init(130);
            list[63] = true;
            list[65] = true;
            list[66] = true;
            list[65] = false;
            list[129] = true;
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
    }
}
