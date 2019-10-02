using CapnpC.CSharp.Generator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CapnpC.CSharp.MsBuild.Generation
{
    public class CsFileGeneratorResult
    {
        public CsFileGeneratorResult(FileGenerationResult generatorResult, string fileName, IReadOnlyList<CapnpMessage> messages)
        {
            if (generatorResult == null)
            {
                throw new ArgumentNullException(nameof(generatorResult));
            }

            Filename = fileName ?? throw new ArgumentNullException(nameof(fileName));

            Error = generatorResult.Exception?.Message;
            GeneratedCode = generatorResult.GeneratedContent;
            Messages = messages;
        }

        public CsFileGeneratorResult(string error)
        {
            Error = error;
        }

        public CsFileGeneratorResult(string error, IReadOnlyList<CapnpMessage> messages)
        {
            Error = error;
            Messages = messages;
        }

        /// <summary>
        /// The error, if any.
        /// </summary>
        public string Error { get; }

        /// <summary>
        /// The generated code.
        /// </summary>
        public string GeneratedCode { get; }

        public IReadOnlyList<CapnpMessage> Messages { get; }

        public bool Success => Error == null;

        public string Filename { get; }
    }
}