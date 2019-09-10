using System;
using System.Collections.Generic;

namespace CapnpC.CSharp.Generator
{
    /// <summary>
    /// Represents a .capnp -> code generator result
    /// </summary>
    public class GenerationResult
    {
        /// <summary>
        /// Constructs an instance in case of at least partially successful generation.
        /// </summary>
        /// <param name="generatedFiles">Generation result per file to generate</param>
        public GenerationResult(IReadOnlyList<FileGenerationResult> generatedFiles)
        {
            GeneratedFiles = generatedFiles;
        }

        /// <summary>
        /// Constructs an instance in case of total failure.
        /// </summary>
        /// <param name="exception">Exception with details on error</param>
        public GenerationResult(Exception exception)
        {
            Exception = exception;
        }

        /// <summary>
        /// Generation result per file to generate or null in case of total failure
        /// </summary>
        public IReadOnlyList<FileGenerationResult> GeneratedFiles { get; }

        /// <summary>
        /// Exception with details on error or null in case of success
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// true iff generation was successful
        /// </summary>
        public bool IsSuccess => GeneratedFiles != null;

        /// <summary>
        /// Messages read from standard error. Valid for both failure and success (capnp might spit out some warnings).
        /// </summary>
        public IReadOnlyList<CapnpMessage> Messages { get; internal set; }

        /// <summary>
        /// Error classification (if any error)
        /// </summary>
        public CapnpProcessFailure ErrorCategory { get; internal set; }
    }
}
