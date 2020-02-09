namespace Capnp.Rpc
{
    /// <summary>
    /// Low-level capability which as imported from a remote peer.
    /// </summary>
    class ImportedCapability : RemoteCapability
    {
        readonly uint _remoteId;

        public ImportedCapability(IRpcEndpoint ep, uint remoteId): base(ep)
        {
            _remoteId = remoteId;
        }

        protected override void ReleaseRemotely()
        {
            _ep.ReleaseImport(_remoteId);
        }

        protected override Call.WRITER SetupMessage(DynamicSerializerState args, ulong interfaceId, ushort methodId)
        {
            var call = base.SetupMessage(args, interfaceId, methodId);
            call.Target.which = MessageTarget.WHICH.ImportedCap;
            call.Target.ImportedCap = _remoteId;

            return call;
        }

        internal override void Freeze(out IRpcEndpoint boundEndpoint)
        {
            boundEndpoint = _ep;
        }

        internal override void Unfreeze()
        {
        }

        internal override void Export(IRpcEndpoint endpoint, CapDescriptor.WRITER capDesc)
        {
            if (endpoint == _ep)
            {
                capDesc.which = CapDescriptor.WHICH.ReceiverHosted;
                capDesc.ReceiverHosted = _remoteId;
            }
            else
            {
                capDesc.which = CapDescriptor.WHICH.SenderHosted;
                capDesc.SenderHosted = endpoint.AllocateExport(Vine.Create(this), out var _);
            }
        }
    }
}