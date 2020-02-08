using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EchoServiceCapnp.Services
{
    public class CapnpEchoService : CapnpGen.IEchoer
    {
        public void Dispose()
        {
        }

        public Task<IReadOnlyList<byte>> Echo(IReadOnlyList<byte> input, CancellationToken cancellationToken_ = default)
        {
            return Task.FromResult(input);
        }
    }
}
