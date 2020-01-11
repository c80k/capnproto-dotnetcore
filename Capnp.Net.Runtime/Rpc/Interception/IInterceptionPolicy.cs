using System;

namespace Capnp.Rpc.Interception
{
    /// <summary>
    /// An interception policy implements callbacks for outgoing calls and returning forwarded calls.
    /// </summary>
    public interface IInterceptionPolicy: IEquatable<IInterceptionPolicy>
    {
        /// <summary>
        /// A caller ("Alice") initiated a new call, which is now intercepted.
        /// </summary>
        /// <param name="callContext">Context object</param>
        void OnCallFromAlice(CallContext callContext);

        /// <summary>
        /// Given that the intercepted call was forwarded, it returned now from the target ("Bob")
        /// and may (or may not)  be returned to the original caller ("Alice").
        /// </summary>
        /// <param name="callContext"></param>
        void OnReturnFromBob(CallContext callContext);
    }
}