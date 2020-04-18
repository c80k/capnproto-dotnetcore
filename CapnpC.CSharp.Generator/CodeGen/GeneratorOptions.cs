namespace CapnpC.CSharp.Generator.CodeGen
{
    public class GeneratorOptions
    {
        public string TopNamespaceName { get; set; } = "CapnpGen";
        public string ReaderStructName { get; set; } = "READER";
        public string WriterStructName { get; set; } = "WRITER";
        public string ReaderParameterName { get; set; } = "reader";
        public string WriterParameterName { get; set; } = "writer";
        public string ReaderCreateMethodName { get; set; } = "create";
        public string ReaderContextFieldName { get; set; } = "ctx";
        public string ContextParameterName { get; set; } = "ctx";
        public string GroupReaderContextArgName { get; set; } = "ctx";
        public string UnionDiscriminatorEnumName { get; set; } = "WHICH";
        public string UnionDiscriminatorPropName { get; set; } = "which";
        public string UnionDiscriminatorFieldName { get; set; } = "_which";
        public string UnionDiscriminatorUndefinedName { get; set; } = "undefined";
        public string UnionContentFieldName { get; set; } = "_content";
        public string SerializeMethodName { get; set; } = "serialize";
        public string ApplyDefaultsMethodName { get; set; } = "applyDefaults";
        public string AnonymousParameterName { get; set; } = "arg_";
        public string CancellationTokenParameterName { get; set; } = "cancellationToken_";
        public string ParamsLocalName { get; set; } = "in_";
        public string DeserializerLocalName { get; set; } = "d_";
        public string SerializerLocalName { get; set; } = "s_";
        public string ResultLocalName { get; set; } = "r_";
        public string ParamsStructFormat { get; set; } = "Params_{0}";
        public string ResultStructFormat { get; set; } = "Result_{0}";
        public string PropertyNamedLikeTypeRenameFormat { get; set; } = "The{0}";
        public string GenericTypeParameterFormat { get; set; } = "T{0}";
        public string PipeliningExtensionsClassFormat { get; set; } = "PipeliningSupportExtensions_{0}";
        public string ProxyClassFormat { get; set; } = "{0}_Proxy";
        public string SkeletonClassFormat { get; set; } = "{0}_Skeleton";
        public string MemberAccessPathNameFormat { get; set; } = "Path_{0}_{1}_{2}_{3}";
        public string TaskParameterName { get; set; } = "task";
        public string TypeIdFieldName { get; set; } = "typeId";
        public string AwaitProxyName { get; set; } = "AwaitProxy";
        public bool NullableEnableDefault { get; set; } = false;
        public string GeneratorToolName { get; set; } = "capnpc-csharp";
        public string GeneratorToolVersion = ThisAssembly.AssemblyVersion;
    }
}
