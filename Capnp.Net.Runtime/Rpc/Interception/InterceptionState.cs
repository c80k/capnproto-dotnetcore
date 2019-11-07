namespace Capnp.Rpc.Interception
{
    /// <summary>
    /// The state of an intercepted call from Alice to Bob.
    /// </summary>
    public enum InterceptionState
    {
        /// <summary>
        /// Alice initiated the call, but it was neither forwarded to Bob nor finished.
        /// </summary>
        RequestedFromAlice,

        /// <summary>
        /// The call was forwarded to Bob.
        /// </summary>
        ForwardedToBob,

        /// <summary>
        /// The call returned from Bob (to whom it was forwarded), but no result was yet forwarded to Alice.
        /// </summary>
        ReturnedFromBob,

        /// <summary>
        /// The call was returned to Alice (either with results, exception, or cancelled)
        /// </summary>
        ReturnedToAlice
    }
}
