using System;
using Microsoft.Extensions.Logging;

#nullable enable
namespace Capnp
{
    /// <summary>
    /// Runtime logging features rely on <see cref="Microsoft.Extensions.Logging"/>
    /// </summary>
    public static class Logging
    {
        /// <summary>
        /// Gets or sets the logger factory which will be used by this assembly.
        /// </summary>
        public static ILoggerFactory LoggerFactory { get; set; } = new LoggerFactory();

        /// <summary>
        /// Creates a new ILogger instance, using the LoggerFactory of this class.
        /// </summary>
        /// <typeparam name="T">The type using the logger</typeparam>
        /// <returns>The logger instance</returns>
        public static ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
    }
}
#nullable restore