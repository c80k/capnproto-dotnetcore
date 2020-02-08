using BenchmarkDotNet.Running;
using Capnp.Rpc;
using Grpc.Net.Client;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<GrpcBenchmark>();
            BenchmarkRunner.Run<CapnpBenchmark>();
        }
    }
}
