using System;

namespace Capnp.Rpc
{
    /// <summary>
    /// Base class for a low-level capability at consumer side. It is created by the <see cref="RpcEngine"/>. An application does not directly interact with it
    /// (which is intentionally impossible, since the invocation method is internal), but instead uses a <see cref="Proxy"/>-derived wrapper.
    /// </summary>
    public abstract class ConsumedCapability
    {
        internal abstract IPromisedAnswer DoCall(ulong interfaceId, ushort methodId, DynamicSerializerState args);

        /// <summary>
        /// Request the RPC engine to release this capability from its import table, 
        /// which usually also means to remove it from the remote peer's export table.
        /// </summary>
        protected abstract void ReleaseRemotely();
        internal abstract Action? Export(IRpcEndpoint endpoint, CapDescriptor.WRITER writer);
        internal abstract void Freeze(out IRpcEndpoint? boundEndpoint);
        internal abstract void Unfreeze();

        internal abstract void AddRef();
        internal abstract void Release(
            bool keepAlive,
            [System.Runtime.CompilerServices.CallerMemberName] string methodName = "", 
            [System.Runtime.CompilerServices.CallerFilePath] string filePath = "", 
            [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0);
    }
}