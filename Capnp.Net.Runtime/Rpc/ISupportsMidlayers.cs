using System;
using System.IO;

namespace Capnp.Rpc
{
    /// <summary>
    /// Common interface for classes supporting the installation of midlayers.
    /// A midlayer is a protocal layer that resides somewhere between capnp serialization and the raw TCP stream.
    /// Thus, we have a hook mechanism for transforming data before it is sent to the TCP connection or after it was received
    /// by the TCP connection, respectively. This mechanism can be used for buffering, various (de-)compression algorithms, and more.
    /// </summary>
    public interface ISupportsMidlayers
    {
        /// <summary>
        /// Installs a midlayer
        /// </summary>
        /// <param name="createFunc">Callback for wrapping the midlayer around its underlying stream</param>
        /// <exception cref="ArgumentNullException"><paramref name="createFunc"/> is null</exception>
        void InjectMidlayer(Func<Stream, Stream> createFunc);
    }
}