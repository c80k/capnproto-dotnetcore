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
    [TestCategory("Coverage")]
    public class LocalRpc: TestBase
    {
        [TestMethod]
        public void DeferredLocalAnswer()
        {
            var tcs = new TaskCompletionSource<int>();
            var impl = new TestPipelineImpl2(tcs.Task);
            var bproxy = BareProxy.FromImpl(impl);
            using (var proxy = bproxy.Cast<ITestPipeline>(true))
            using (var cap = proxy.GetCap(0, null).OutBox_Cap())
            {
                var foo = cap.Foo(123, true);
                tcs.SetResult(0);
                Assert.IsTrue(foo.Wait(TestBase.MediumNonDbgTimeout));
                Assert.AreEqual("bar", foo.Result);
            }
        }

        [TestMethod]
        public void Embargo()
        {
            NewLocalTestbed().RunTest(Testsuite.Embargo);
        }

        [TestMethod]
        public void EmbargoError()
        {
            NewLocalTestbed().RunTest(Testsuite.EmbargoError);
        }

        [TestMethod]
        public void EmbargoNull()
        {
            NewLocalTestbed().RunTest(Testsuite.EmbargoNull);
        }

        [TestMethod]
        public void CallBrokenPromise()
        {
            NewLocalTestbed().RunTest(Testsuite.CallBrokenPromise);
        }

        [TestMethod]
        public void TailCall()
        {
            NewLocalTestbed().RunTest(Testsuite.TailCall);
        }

        [TestMethod]
        public void SendTwice()
        {
            NewLocalTestbed().RunTest(Testsuite.SendTwice);
        }

        [TestMethod]
        public void Cancel()
        {
            NewLocalTestbed().RunTest(Testsuite.Cancel);
        }

        [TestMethod]
        public void RetainAndRelease()
        {
            NewLocalTestbed().RunTest(Testsuite.RetainAndRelease);
        }

        [TestMethod]
        public void PromiseResolve()
        {
            NewLocalTestbed().RunTest(Testsuite.PromiseResolve);
        }

        [TestMethod]
        public void Cancelation()
        {
            NewLocalTestbed().RunTest(Testsuite.Cancelation);
        }

        [TestMethod]
        public void ReleaseOnCancel()
        {
            NewLocalTestbed().RunTest(Testsuite.ReleaseOnCancel);
        }

        [TestMethod]
        public void Release()
        {
            NewLocalTestbed().RunTest(Testsuite.Release);
        }

        [TestMethod]
        public void Pipeline()
        {
            NewLocalTestbed().RunTest(Testsuite.Pipeline);
        }

        [TestMethod]
        public void Basic()
        {
            NewLocalTestbed().RunTest(Testsuite.Basic);
        }

        [TestMethod]
        public void Ownership1()
        {
            NewLocalTestbed().RunTest(Testsuite.Ownership1);
        }

        [TestMethod]
        public void Ownership2()
        {
            NewLocalTestbed().RunTest(Testsuite.Ownership2);
        }

        [TestMethod]
        public void Ownership3()
        {
            NewLocalTestbed().RunTest(Testsuite.Ownership3);
        }
    }
}
