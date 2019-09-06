using System;

namespace CapnpC.CSharp.Generator
{
    /// <summary>
    /// Represents the generation result of a single .capnp file
    /// </summary>
    public class FileGenerationResult
    {
        /// <summary>
        /// Constructs an instance in case of successful generation
        /// </summary>
        /// <param name="capnpFilePath">path to .capnp file</param>
        /// <param name="generatedContent">generated C# code</param>
        public FileGenerationResult(string capnpFilePath, string generatedContent)
        {
            CapnpFilePath = capnpFilePath;
            GeneratedContent = generatedContent;
        }

        /// <summary>
        /// Constructs an instance in case of unsuccessful generation
        /// </summary>
        /// <param name="capnpFilePath">path to .capnp file</param>
        /// <param name="exception">Exception giving details on the error which prevented generation</param>
        public FileGenerationResult(string capnpFilePath, Exception exception)
        {
            CapnpFilePath = capnpFilePath;
            Exception = exception;
        }

        /// <summary>
        /// Path to .capnp file
        /// </summary>
        public string CapnpFilePath { get; }

        /// <summary>
        /// Generated C# or null if generation failed
        /// </summary>
        public string GeneratedContent { get; }

        /// <summary>
        /// Exception giving details on the error which prevented generation
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// true iff generation was successful
        /// </summary>
        public bool IsSuccess => !string.IsNullOrEmpty(GeneratedContent);
    }
}
