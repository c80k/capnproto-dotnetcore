using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capnp.Net.Runtime.Tests
{
    [TestClass]
    [TestCategory("Coverage")]
    public class Dtbdct: TestBase
    {
        [TestMethod]
        public void Embargo()
        {
            NewDtbdctTestbed().RunTest(Testsuite.Embargo);
        }

        [TestMethod]
        public void EmbargoError()
        {
            NewDtbdctTestbed().RunTest(Testsuite.EmbargoError);
        }

        [TestMethod]
        public void EmbargoNull()
        {
            NewDtbdctTestbed().RunTest(Testsuite.EmbargoNull);
        }

        [TestMethod]
        public void CallBrokenPromise()
        {
            NewDtbdctTestbed().RunTest(Testsuite.CallBrokenPromise);
        }

        [TestMethod]
        public void TailCall()
        {
            NewDtbdctTestbed().RunTest(Testsuite.TailCall);
        }

        [TestMethod]
        public void SendTwice()
        {
            NewDtbdctTestbed().RunTest(Testsuite.SendTwice);
        }

        [TestMethod]
        public void Cancel()
        {
            NewDtbdctTestbed().RunTest(Testsuite.Cancel);
        }

        [TestMethod]
        public void RetainAndRelease()
        {
            NewDtbdctTestbed().RunTest(Testsuite.RetainAndRelease);
        }

        [TestMethod]
        public void PromiseResolve()
        {
            NewDtbdctTestbed().RunTest(Testsuite.PromiseResolve);
        }

        [TestMethod]
        public void Cancelation()
        {
            NewDtbdctTestbed().RunTest(Testsuite.Cancelation);
        }

        [TestMethod]
        public void ReleaseOnCancel()
        {
            NewDtbdctTestbed().RunTest(Testsuite.ReleaseOnCancel);
        }

        [TestMethod]
        public void Release()
        {
            NewDtbdctTestbed().RunTest(Testsuite.Release);
        }

        [TestMethod]
        public void Pipeline()
        {
            NewDtbdctTestbed().RunTest(Testsuite.Pipeline);
        }

        [TestMethod]
        public void Basic()
        {
            NewDtbdctTestbed().RunTest(Testsuite.Basic);
        }

        [TestMethod]
        public void BootstrapReuse()
        {
            NewDtbdctTestbed().RunTest(Testsuite.BootstrapReuse);
        }
    }
}
