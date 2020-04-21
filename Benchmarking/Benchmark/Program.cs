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
            if (args.Length == 0 || args[0] == "grpc")
                BenchmarkRunner.Run<GrpcBenchmark>();

            if (args.Length == 0 || args[0] == "capnp")
                BenchmarkRunner.Run<CapnpBenchmark>();
        }
    }
}
