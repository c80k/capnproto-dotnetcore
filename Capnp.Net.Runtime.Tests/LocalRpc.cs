using Capnp.Net.Runtime.Tests.GenImpls;
using Capnp.Rpc;
using Capnproto_test.Capnp.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Net.Runtime.Tests
{
    [TestClass]
    public class LocalRpc
    {
        [TestMethod]
        public void DeferredLocalAnswer()
        {
            var tcs = new TaskCompletionSource<int>();
            var impl = new TestPipelineImpl2(tcs.Task);
            var bproxy = BareProxy.FromImpl(impl);
            var proxy = bproxy.Cast<ITestPipeline>(true);
            var cap = proxy.GetCap(0, null).OutBox_Cap();
            var foo = cap.Foo(123, true);
            tcs.SetResult(0);
            Assert.IsTrue(foo.Wait(TestBase.MediumNonDbgTimeout));
            Assert.AreEqual("bar", foo.Result);
        }
    }
}
