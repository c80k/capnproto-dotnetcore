using System;

namespace Capnp.Rpc.Interception
{
    public interface IInterceptionPolicy: IEquatable<IInterceptionPolicy>
    {
        void OnCallFromAlice(CallContext callContext);
        void OnReturnFromBob(CallContext callContext);
    }
}
