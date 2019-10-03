using System;
using System.Collections.Generic;
using System.Text;

namespace Capnp.FrameTracing
{
    public enum FrameDirection
    {
        Rx,
        Tx
    }

    public interface IFrameTracer: IDisposable
    {
        void TraceFrame(FrameDirection direction, WireFrame frame);
    }
}
