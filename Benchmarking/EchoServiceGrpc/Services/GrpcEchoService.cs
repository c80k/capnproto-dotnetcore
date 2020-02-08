using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace EchoService
{
    public class GrpcEchoService : Echoer.EchoerBase
    {
        private readonly ILogger<GrpcEchoService> _logger;
        public GrpcEchoService(ILogger<GrpcEchoService> logger)
        {
            _logger = logger;
        }

        public override Task<EchoReply> Echo(EchoRequest request, ServerCallContext context)
        {
            return Task.FromResult(new EchoReply
            {
                Payload = request.Payload
            });
        }
    }
}
