using Capnp.Rpc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Capnp.Net.Runtime.Tests
{
    [TestClass]
    public class General
    {
        [TestMethod]
        public void AwaitOrderTest()
        {
            // This test verifies an execution order assumption about the .NET environment:
            // When I register multiple continuations on the same Task, using the await
            // keyword, I expect all continuations be executed in the same order they were
            // registered. Despite I could not find any official statement on this behavior,
            // the Capnp.Net.Runtime implementation relies on that assumption. Should that
            // assumption turn out to be wrong, you might observe RPCs which are executed in
            // a different order than they were requested.

            int returnCounter = 0;

            async Task ExpectCount(Task task, int count)
            {
                await task;
                Assert.AreEqual(count, returnCounter++);
            }

            var tcs = new TaskCompletionSource<int>();

            var tasks =
                from i in Enumerable.Range(0, 100)
                select ExpectCount(tcs.Task, i);

            tcs.SetResult(0);

            Task.WhenAll(tasks).Wait();
        }

        class PromisedAnswerMock : IPromisedAnswer
        {
            readonly TaskCompletionSource<DeserializerState> _tcs = new TaskCompletionSource<DeserializerState>();

            public Task<DeserializerState> WhenReturned => _tcs.Task;

            public void Return()
            {
                _tcs.SetResult(default);
            }

            public void Cancel()
            {
                _tcs.SetCanceled();
            }

            public void Fault()
            {
                _tcs.SetException(new InvalidOperationException("test fault"));
            }

            public ConsumedCapability Access(MemberAccessPath access)
            {
                throw new NotImplementedException();
            }

            public ConsumedCapability Access(MemberAccessPath access, Task<IDisposable> proxyTask)
            {
                throw new NotImplementedException();
            }

            public bool IsTailCall => false;

            public void Dispose()
            {
            }
        }

        [TestMethod]
        public void MakePipelineAwareOnFastPath()
        {
            var mock = new PromisedAnswerMock();
            mock.Return();
            for (int i = 0; i < 100; i++)
            {
                var t = Impatient.MakePipelineAware(mock, _ => (object)null);
                Assert.IsTrue(t.IsCompleted);
            };
        }
    }
}
