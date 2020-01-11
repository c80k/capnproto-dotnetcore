#nullable enable
namespace Capnp.Rpc
{
    static class ResolvingCapabilityExtensions
    {
        public static void ExportAsSenderPromise<T>(this T cap, IRpcEndpoint endpoint, CapDescriptor.WRITER writer)
            where T: ConsumedCapability, IResolvingCapability
        {
            var vine = Vine.Create(cap);
            uint preliminaryId = endpoint.AllocateExport(vine, out bool first);

            writer.which = CapDescriptor.WHICH.SenderPromise;
            writer.SenderPromise = preliminaryId;

            if (first)
            {
                endpoint.RequestPostAction(async () => {

                    try
                    {
                        var resolvedCap = await cap.WhenResolved;

                        endpoint.Resolve(preliminaryId, vine, () => resolvedCap.ConsumedCap!);
                    }
                    catch (System.Exception exception)
                    {
                        endpoint.Resolve(preliminaryId, vine, () => throw exception);
                    }

                });
            }
        }

        public static async void DisposeWhenResolved(this IResolvingCapability cap)
        {
            try
            {
                (await cap.WhenResolved)?.Dispose();
            }
            catch
            {
            }
        }
    }
}
#nullable restore