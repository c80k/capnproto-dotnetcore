using System;

namespace Capnp.FrameTracing
{
    /// <summary>
    /// Send or receive
    /// </summary>
    public enum FrameDirection
    {
        /// <summary>
        /// Receive direction
        /// </summary>
        Rx,

        /// <summary>
        /// Send direction
        /// </summary>
        Tx
    }

    /// <summary>
    /// Client interface for observing RPC traffic
    /// </summary>
    public interface IFrameTracer: IDisposable
    {
        /// <summary>
        /// Called whenever an RPC frame was sent or received
        /// </summary>
        /// <param name="direction">frame direction</param>
        /// <param name="frame">actual frame</param>
        void TraceFrame(FrameDirection direction, WireFrame frame);
    }
}
