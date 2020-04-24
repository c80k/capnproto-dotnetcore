using System;

namespace CapnpC.CSharp.Generator.CodeGen
{
    abstract class SkeletonWorder : Capnp.Rpc.Skeleton<object>
    {
        public const string SetMethodTableName = nameof(SkeletonWorder.SetMethodTable);
        public const string ImplName = nameof(SkeletonWorder.Impl);
    }
}
