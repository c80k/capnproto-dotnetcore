namespace Capnp.Rpc.Interception
{
    class CensorCapability : RefCountingCapability
    {
        public CensorCapability(ConsumedCapability interceptedCapability, IInterceptionPolicy policy)
        {
            InterceptedCapability = interceptedCapability;
            interceptedCapability.AddRef();
            Policy = policy;
            MyVine = Vine.Create(this);
        }

        public ConsumedCapability InterceptedCapability { get; }
        public IInterceptionPolicy Policy { get; }
        internal Skeleton MyVine { get; }

        protected override void ReleaseRemotely()
        {
            InterceptedCapability.Release(false);
        }

        internal override IPromisedAnswer DoCall(ulong interfaceId, ushort methodId, DynamicSerializerState args)
        {
            var cc = new CallContext(this, interfaceId, methodId, args);
            Policy.OnCallFromAlice(cc);
            return cc.Answer;
        }

        internal override void Export(IRpcEndpoint endpoint, CapDescriptor.WRITER writer)
        {
            writer.which = CapDescriptor.WHICH.SenderHosted;
            writer.SenderHosted = endpoint.AllocateExport(MyVine, out bool _);
        }

        internal override void Freeze(out IRpcEndpoint? boundEndpoint)
        {
            boundEndpoint = null;
        }

        internal override void Unfreeze()
        {
        }
    }
}