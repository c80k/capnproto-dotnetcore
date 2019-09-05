using System;
using System.Collections.Generic;
using System.Linq;

namespace Capnpc.Csharp.MsBuild.Generation
{
    public class TestFileGeneratorResult
    {
        public TestFileGeneratorResult(TestGeneratorResult generatorResult, string fileName)
        {
            if (generatorResult == null)
            {
                throw new ArgumentNullException(nameof(generatorResult));
            }

            Filename = fileName ?? throw new ArgumentNullException(nameof(fileName));

            Errors = generatorResult.Errors;
            IsUpToDate = generatorResult.IsUpToDate;
            GeneratedCode = generatorResult.GeneratedTestCode;
        }

        /// <summary>
        /// The errors, if any.
        /// </summary>
        public IEnumerable<TestGenerationError> Errors { get; }

        /// <summary>
        /// The generated file was up-to-date.
        /// </summary>
        public bool IsUpToDate { get; }

        /// <summary>
        /// The generated code.
        /// </summary>
        public string GeneratedCode { get; }

        public bool Success => Errors == null || !Errors.Any();

        public string Filename { get; }
    }
}