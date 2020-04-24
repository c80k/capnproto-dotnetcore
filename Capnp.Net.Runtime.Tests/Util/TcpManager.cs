using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Capnp.Net.Runtime.Tests.Util
{
    class TcpManager
    {
        public static readonly TcpManager Instance = new TcpManager();

        readonly byte[] _nextAddress;
        int _nextPort = 50005;

        public TcpManager()
        {
            _nextAddress = new byte[] { 127, 0, 0, 1 };
        }

        public (IPAddress, int) GetLocalAddressAndPort()
        {
            if (++_nextAddress[2] == 0 &&
                ++_nextAddress[1] == 0 &&
                ++_nextAddress[0] == 0)
            {
                _nextAddress[0] = 2;
            }

            return (new IPAddress(_nextAddress), _nextPort);
        }
    }
}
