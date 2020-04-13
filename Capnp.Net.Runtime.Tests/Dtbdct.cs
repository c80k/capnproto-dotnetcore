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
        public void EmbargoOnPromisedAnswer()
        {
            NewDtbdctTestbed().RunTest(Testsuite.EmbargoOnPromisedAnswer);
        }

        [TestMethod]
        public void EmbargoOnImportedCap()
        {
            NewDtbdctTestbed().RunTest(Testsuite.EmbargoOnImportedCap);
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
        public void PromiseResolveLate()
        {
            NewDtbdctTestbed().RunTest(Testsuite.PromiseResolveLate);
        }

        [TestMethod]
        public void PromiseResolveError()
        {
            NewDtbdctTestbed().RunTest(Testsuite.PromiseResolveError);
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

        [TestMethod]
        public void Ownership1()
        {
            NewDtbdctTestbed().RunTest(Testsuite.Ownership1);
        }

        [TestMethod]
        public void Ownership2()
        {
            NewDtbdctTestbed().RunTest(Testsuite.Ownership2);
        }

        [TestMethod]
        public void Ownership3()
        {
            NewDtbdctTestbed().RunTest(Testsuite.Ownership3);
        }

        [TestMethod]
        public void SillySkeleton()
        {
            NewDtbdctTestbed().RunTest(Testsuite.SillySkeleton);
        }

        [TestMethod]
        public void ImportReceiverAnswer()
        {
            NewDtbdctTestbed().RunTest(Testsuite.ImportReceiverAnswer);
        }

        [TestMethod]
        public void ImportReceiverAnswerError()
        {
            NewDtbdctTestbed().RunTest(Testsuite.ImportReceiverAnswerError);
        }

        [TestMethod]
        public void ImportReceiverAnswerCanceled()
        {
            NewDtbdctTestbed().RunTest(Testsuite.ImportReceiverCanceled);
        }

        [TestMethod]
        public void ButNoTailCall()
        {
            NewDtbdctTestbed().RunTest(Testsuite.ButNoTailCall);
        }

        [TestMethod]
        public void SecondIsTailCall()
        {
            NewDtbdctTestbed().RunTest(Testsuite.SecondIsTailCall);
        }

        [TestMethod]
        public void ReexportSenderPromise()
        {
            NewDtbdctTestbed().RunTest(Testsuite.ReexportSenderPromise);
        }

        [TestMethod]
        public void CallAfterFinish1()
        {
            NewDtbdctTestbed().RunTest(Testsuite.CallAfterFinish1);
        }

        [TestMethod]
        public void CallAfterFinish2()
        {
            NewDtbdctTestbed().RunTest(Testsuite.CallAfterFinish2);
        }
    }
}
