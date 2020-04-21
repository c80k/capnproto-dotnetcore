using Capnp;
using Capnp.Rpc;
using System.Collections.Generic;

namespace CapnpProfile
{
    class EnginePair
    {
        class EngineChannel : IEndpoint
        {
            readonly Queue<WireFrame> _frameBuffer = new Queue<WireFrame>();
            bool _dismissed;

            public EngineChannel()
            {
            }

            public RpcEngine.RpcEndpoint OtherEndpoint { get; set; }
            public bool HasBufferedFrames => _frameBuffer.Count > 0;
            public int FrameCounter { get; private set; }


            public void Dismiss()
            {
                if (!_dismissed)
                {
                    _dismissed = true;
                    OtherEndpoint.Dismiss();
                }
            }

            public void Forward(WireFrame frame)
            {
                if (_dismissed)
                    return;

                OtherEndpoint.Forward(frame);
            }
        }

        readonly EngineChannel _channel1, _channel2;

        public RpcEngine Engine1 { get; }
        public RpcEngine Engine2 { get; }
        public RpcEngine.RpcEndpoint Endpoint1 { get; }
        public RpcEngine.RpcEndpoint Endpoint2 { get; }

        public EnginePair()
        {
            Engine1 = new RpcEngine();
            Engine2 = new RpcEngine();
            _channel1 = new EngineChannel();
            Endpoint1 = Engine1.AddEndpoint(_channel1);
            _channel2 = new EngineChannel();
            Endpoint2 = Engine2.AddEndpoint(_channel2);
            _channel1.OtherEndpoint = Endpoint2;
            _channel2.OtherEndpoint = Endpoint1;
        }

        public int Channel1SendCount => _channel1.FrameCounter;
        public int Channel2SendCount => _channel2.FrameCounter;
    }
}
