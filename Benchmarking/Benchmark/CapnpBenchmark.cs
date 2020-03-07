using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Capnp.Rpc;
using CapnpGen;
using System;
using System.Collections.Generic;
using System.Text;

namespace Benchmark
{
    public class CapnpBenchmark
    {
        [Params(20, 200, 2000, 20000, 200000, 2000000)]
        public int PayloadBytes;

        [Params(0, 256, 1024, 4096)]
        public int BufferSize;

        TcpRpcClient _client;
        IEchoer _echoer;
        byte[] _payload;

        [GlobalSetup]
        public void Setup()
        {
            _client = new TcpRpcClient("localhost", 5002);
            if (BufferSize > 0)
                _client.AddBuffering(BufferSize);
            _client.WhenConnected.Wait();
            _echoer = _client.GetMain<IEchoer>();
            _payload = new byte[PayloadBytes];
            new Random().NextBytes(_payload);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _echoer.Dispose();
            _client.Dispose();
        }

        [Benchmark]
        public void Echo()
        {
            var t = _echoer.Echo(_payload);
            t.Wait();
            if (t.Result?.Count != _payload.Length)
                throw new InvalidOperationException("Echo server malfunction");
        }
    }
}
