namespace CapnpC.CSharp.Generator
{
    /// <summary>
    /// Why did invocation of capnpc.exe fail?
    /// </summary>
    public enum CapnpProcessFailure
    {
        /// <summary>
        /// Because capnpc.exe was not found. It is probably not installed.
        /// </summary>
        NotFound,

        /// <summary>
        /// Because it exited with an error. Probably invalid .capnp file input.
        /// </summary>
        BadInput,

        /// <summary>
        /// Because it produced an apparently bad code generation request.
        /// </summary>
        BadOutput
    }
}
