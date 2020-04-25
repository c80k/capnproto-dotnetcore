namespace CapnpC.CSharp.Generator.CodeGen
{
    /// <summary>
    /// Provides options properties defining how the generated code will look like.
    /// </summary>
    public class GeneratorOptions
    {
        /// <summary>
        /// Default namespace if .capnp file does not specify any.
        /// </summary>
        public string TopNamespaceName { get; set; } = "CapnpGen";

        /// <summary>
        /// Type name of generated struct reader
        /// </summary>
        public string ReaderStructName { get; set; } = "READER";
        
        /// <summary>
        /// Type name of generated struct writer
        /// </summary>
        public string WriterStructName { get; set; } = "WRITER";

        /// <summary>
        /// Parameter name when struct reader is passed as argument
        /// </summary>
        public string ReaderParameterName { get; set; } = "reader";

        /// <summary>
        /// Parameter name when struct writer is passed as argument
        /// </summary>
        public string WriterParameterName { get; set; } = "writer";

        /// <summary>
        /// Struct reader creation method name
        /// </summary>
        public string ReaderCreateMethodName { get; set; } = "create";

        /// <summary>
        /// Name of struct reader's underlying DeserializerState field 
        /// </summary>
        public string ReaderContextFieldName { get; set; } = "ctx";

        /// <summary>
        /// Name of struct writer's underlying SerializerState field
        /// </summary>
        public string ContextParameterName { get; set; } = "ctx";

        /// <summary>
        /// Name of group reader's underlying DeserializerState field
        /// </summary>
        public string GroupReaderContextArgName { get; set; } = "ctx";

        /// <summary>
        /// Name of union discriminator enum
        /// </summary>
        public string UnionDiscriminatorEnumName { get; set; } = "WHICH";

        /// <summary>
        /// Name of union discriminator property
        /// </summary>
        public string UnionDiscriminatorPropName { get; set; } = "which";
        
        /// <summary>
        /// Name of private union dicriminator field
        /// </summary>
        public string UnionDiscriminatorFieldName { get; set; } = "_which";

        /// <summary>
        /// Literal name for undetermined union discriminators
        /// </summary>
        public string UnionDiscriminatorUndefinedName { get; set; } = "undefined";

        /// <summary>
        /// Name of union content field
        /// </summary>
        public string UnionContentFieldName { get; set; } = "_content";

        /// <summary>
        /// Domain class method name for serializing its contents to the writer class
        /// </summary>
        public string SerializeMethodName { get; set; } = "serialize";

        /// <summary>
        /// Domain class method name for applying default values
        /// </summary>
        public string ApplyDefaultsMethodName { get; set; } = "applyDefaults";

        /// <summary>
        /// Default input-arguments parameter name for interface methods
        /// </summary>
        public string AnonymousParameterName { get; set; } = "arg_";

        /// <summary>
        /// Parameter name for passing a CancellationToken to interface methods
        /// </summary>
        public string CancellationTokenParameterName { get; set; } = "cancellationToken_";

        /// <summary>
        /// Local name for usage in generated proxy/skeleton code.
        /// </summary>
        public string ParamsLocalName { get; set; } = "in_";

        /// <summary>
        /// Local name for usage in generated proxy/skeleton code.
        /// </summary>
        public string DeserializerLocalName { get; set; } = "d_";

        /// <summary>
        /// Local name for usage in generated proxy/skeleton code.
        /// </summary>
        public string SerializerLocalName { get; set; } = "s_";

        /// <summary>
        /// Local name for usage in generated proxy/skeleton code.
        /// </summary>
        public string ResultLocalName { get; set; } = "r_";

        /// <summary>
        /// Pattern for generating method input struct name
        /// </summary>
        public string ParamsStructFormat { get; set; } = "Params_{0}";

        /// <summary>
        /// Pattern for generating method output struct name
        /// </summary>
        public string ResultStructFormat { get; set; } = "Result_{0}";

        /// <summary>
        /// Renaming pattern when a generated property would happen to have the same name like its surrounding type
        /// (which is illegal in C#)
        /// </summary>
        public string PropertyNamedLikeTypeRenameFormat { get; set; } = "The{0}";

        /// <summary>
        /// Pattern for generating generic type parameter names
        /// </summary>
        public string GenericTypeParameterFormat { get; set; } = "T{0}";

        /// <summary>
        /// Pattern for generating pipelining-related support classes
        /// </summary>
        public string PipeliningExtensionsClassFormat { get; set; } = "PipeliningSupportExtensions_{0}";

        /// <summary>
        /// Pattern for generating proxy class name
        /// </summary>
        public string ProxyClassFormat { get; set; } = "{0}_Proxy";

        /// <summary>
        /// Pattern for generating skeleton class name
        /// </summary>
        public string SkeletonClassFormat { get; set; } = "{0}_Skeleton";

        /// <summary>
        /// Pattern for generating member access path objects used in pipelining-related support classes
        /// </summary>
        public string MemberAccessPathNameFormat { get; set; } = "Path_{0}_{1}_{2}_{3}";

        /// <summary>
        /// Parameter name for passing a Task
        /// </summary>
        public string TaskParameterName { get; set; } = "task";

        /// <summary>
        /// Field name for type ID
        /// </summary>
        public string TypeIdFieldName { get; set; } = "typeId";

        /// <summary>
        /// Local method name used in generated code.
        /// </summary>
        public string AwaitProxyName { get; set; } = "AwaitProxy";

        /// <summary>
        /// Whether to generate nullable reference types if not explicitly specified
        /// </summary>
        public bool NullableEnableDefault { get; set; } = false;

        /// <summary>
        /// Generator tool name for GeneratedCodeAttribute
        /// </summary>
        public string GeneratorToolName { get; set; } = "capnpc-csharp";

        /// <summary>
        /// Generator tool version for GeneratedCodeAttribute
        /// </summary>
        public string GeneratorToolVersion = ThisAssembly.AssemblyVersion;
    }
}
