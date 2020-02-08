using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Benchmark
{
    public class GrpcBenchmark
    {
        [Params(20, 200, 2000, 20000, 200000, 2000000)]
        public int PayloadBytes;

        GrpcChannel _channel;
        Echoer.EchoerClient _echoer;
        byte[] _payload;

        [GlobalSetup]
        public void Setup()
        {
            _channel = GrpcChannel.ForAddress("https://localhost:5001");
            _echoer = new Echoer.EchoerClient(_channel);
            _payload = new byte[PayloadBytes];
            new Random().NextBytes(_payload);
        }

        [GlobalCleanup]
        public void Teardown()
        {
            _channel.Dispose();
        }

        [Benchmark]
        public void Echo()
        {
            var reply = _echoer.Echo(new EchoRequest { Payload = Google.Protobuf.ByteString.CopyFrom(_payload) });
            if (reply?.Payload?.Length != _payload.Length)
                throw new InvalidOperationException("Echo server malfunction");
        }
    }
}
