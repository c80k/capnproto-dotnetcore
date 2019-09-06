using Capnp;
using System;
using System.Diagnostics;
using System.IO;

namespace CapnpC.CSharp.Generator
{
    /// <summary>
    /// Provides methods for controlling both the C# code generator backend and the frontend "capnpc"
    /// </summary>
    public static class CapnpCompilation
    {
        /// <summary>
        /// Generates C# code from given input stream
        /// </summary>
        /// <param name="input">input stream containing the binary code generation request, which the frontend capnpc emits</param>
        /// <returns>generation result</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="input"/> is null</exception>
        public static GenerationResult GenerateFromStream(Stream input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            try
            {
                WireFrame segments;

                using (input)
                {
                    segments = Framing.ReadSegments(input);
                }

                var dec = DeserializerState.CreateRoot(segments);
                var reader = Schema.CodeGeneratorRequest.Reader.Create(dec);
                var model = Model.SchemaModel.Create(reader);
                var codeGen = new CodeGen.CodeGenerator(model, new CodeGen.GeneratorOptions());
                return new GenerationResult(codeGen.Generate());
            }
            catch (Exception exception)
            {
                return new GenerationResult(exception);
            }
        }

        public static GenerationResult InvokeCapnpcAndGenerate()
        {
        }
    }
}
