#nullable enable
namespace Capnp
{
    /// <summary>
    /// Provides the security bounds for defeating amplification and stack overflow DoS attacks.
    /// See https://capnproto.org/encoding.html#security-considerations for details.
    /// </summary>
    public static class SecurityOptions
    {
        /// <summary>
        /// The traversal limit, see https://capnproto.org/encoding.html#amplification-attack
        /// </summary>
        public static uint TraversalLimit { get; set; } = 64 * 1024 * 1024;
        /// <summary>
        /// The recursion limit, see https://capnproto.org/encoding.html#stack-overflow-dos-attack
        /// </summary>
        public static int RecursionLimit { get; set; } = 64;
    }
}
#nullable restore