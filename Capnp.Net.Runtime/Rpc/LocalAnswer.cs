using System;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Rpc
{
    class LocalAnswer : IPromisedAnswer
    {
        readonly CancellationTokenSource _cts;

        public LocalAnswer(CancellationTokenSource cts, Task<DeserializerState> call)
        {
            _cts = cts ?? throw new ArgumentNullException(nameof(cts));
            WhenReturned = call ?? throw new ArgumentNullException(nameof(call));

            CleanupAfterReturn();
        }

        async void CleanupAfterReturn()
        {
            try
            {
                await WhenReturned;
            }
            catch
            {
            }
            finally
            {
                _cts.Dispose();
            }
        }

        public Task<DeserializerState> WhenReturned { get; }

        public ConsumedCapability Access(MemberAccessPath access)
        {
            return new LocalAnswerCapability(WhenReturned, access);
        }

        public void Dispose()
        {
            try
            {
                _cts.Cancel();
            }
            catch (ObjectDisposedException)
            {
            }
        }
    }
}