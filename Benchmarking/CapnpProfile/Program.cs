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
        static async Task Main(string[] args)
        {
            using var server = new TcpRpcServer(IPAddress.Any, 5002);
            server.Main = new CapnpEchoService();
            using var client = new TcpRpcClient("localhost", 5002);
            await client.WhenConnected;
            using var echoer = client.GetMain<IEchoer>();
            var payload = new byte[20];
            new Random().NextBytes(payload);

            while (true)
            {
                var result = await echoer.Echo(payload);
                if (result.Count != payload.Length)
                    throw new InvalidOperationException("Echo server malfunction");
            }
        }
    }
}
