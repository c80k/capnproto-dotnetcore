using System;
using System.Collections.Generic;
using System.Text;

namespace Capnp.Rpc
{
    /// <summary>
    /// Provides extension methods for installing midlayers to <see cref="TcpRpcServer"/> and <see cref="TcpRpcClient"/>./>.
    /// </summary>
    public static class MidlayerExtensions
    {
        /// <summary>
        /// Enables stream buffering on the given object. Stream buffering reduces the number of I/O operations, 
        /// hence may cause a significant performance boost.
        /// </summary>
        /// <param name="obj"><see cref="TcpRpcServer"/> or <see cref="TcpRpcClient"/></param>
        /// <param name="bufferSize">Buffer size (bytes). You should choose it according to the maximum expected raw capnp frame size</param>
        public static void AddBuffering(this ISupportsMidlayers obj, int bufferSize)
        {
            obj.InjectMidlayer(s => new Util.DuplexBufferedStream(s, bufferSize));
        }

        /// <summary>
        /// Enables stream buffering on the given object. Stream buffering reduces the number of I/O operations, 
        /// hence may cause a significant performance boost. Some default buffer size will be chosen.
        /// </summary>
        /// <param name="obj"><see cref="TcpRpcServer"/> or <see cref="TcpRpcClient"/></param>
        public static void AddBuffering(this ISupportsMidlayers obj)
        {
            obj.InjectMidlayer(s => new Util.DuplexBufferedStream(s));
        }
    }
}
