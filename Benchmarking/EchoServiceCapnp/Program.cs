using Capnp.Rpc;
using EchoServiceCapnp.Services;
using System;
using System.Net;

namespace EchoServiceCapnp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var server = new TcpRpcServer(IPAddress.Any, 5002))
            {
                server.AddBuffering();
                server.Main = new CapnpEchoService();
                Console.WriteLine("Press RETURN to stop listening");
                Console.ReadLine();
            }
        }
    }
}
