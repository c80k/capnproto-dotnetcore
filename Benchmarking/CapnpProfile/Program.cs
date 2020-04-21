using Capnp.Rpc;
using CapnpGen;
using CapnpProfile.Services;
using System;
using System.Net;
using System.Threading.Tasks;

namespace CapnpProfile
{

    class Program
    {
        static async Task Run(IEchoer echoer)
        {
            var payload = new byte[20];
            new Random().NextBytes(payload);

#if SOTASK_PERF
            int counter = 0;
#endif

            while (true)
            {
                var result = await echoer.Echo(payload);
                if (result.Count != payload.Length)
                    throw new InvalidOperationException("Echo server malfunction");

#if SOTASK_PERF
                if (++counter == 10000)
                {
                    counter = 0;

                    Console.WriteLine($"StrictlyOrderedTask performance statistics:");
                    Console.WriteLine($"AwaitInternal: max. {Capnp.Util.StrictlyOrderedTaskExtensions.Stats.AwaitInternalMaxOuterIterations} outer iterations");
                    Console.WriteLine($"AwaitInternal: max. {Capnp.Util.StrictlyOrderedTaskExtensions.Stats.AwaitInternalMaxInnerIterations} inner iterations");
                    Console.WriteLine($"OnCompleted: max. {Capnp.Util.StrictlyOrderedTaskExtensions.Stats.OnCompletedMaxSpins} iterations");
                }
#endif
            }
        }

        static async Task Main(string[] args)
        {

            if (args.Length > 0)
            {
                var pair = new EnginePair();
                pair.Engine1.Main = new CapnpEchoService();
                var echoer = (CapabilityReflection.CreateProxy<IEchoer>(pair.Endpoint2.QueryMain()) as IEchoer);

                await Run(echoer);
            }
            else
            {
                using var server = new TcpRpcServer();
                server.Main = new CapnpEchoService();
                server.AddBuffering();
                server.StartAccepting(IPAddress.Any, 5002);
                using var client = new TcpRpcClient();
                client.AddBuffering();
                client.Connect("localhost", 5002);
                await client.WhenConnected;
                using var echoer = client.GetMain<IEchoer>();

                await Run(echoer);
            }
        }
    }
}
