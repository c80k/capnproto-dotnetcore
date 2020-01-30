using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using TechTalk.SpecFlow;

namespace CapnpC.CSharp.Generator.Tests
{
    [Binding]
    public class CodeGeneratorSteps
    {
        Stream _inputStream;
        string _inputSchemaFileName;
        string _inputSchema;
        string _referenceOutputContent;
        bool _nullableGenEnable;
        bool _nullableSupportEnable;

        GenerationResult _result;

        public static Stream LoadResource(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string[] names = assembly.GetManifestResourceNames();
            string urn = Array.Find(names, n => n.EndsWith(name, StringComparison.OrdinalIgnoreCase));
            Assert.IsNotNull(urn, $"Test specification error: {name} does not exist");
            return assembly.GetManifestResourceStream(urn);
        }

        internal static bool IsCapnpInstalled()
        {
            try
            {
                var startInfo = new ProcessStartInfo(CapnpCompilation.CapnpCompilerFilename, "--version");
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;

                using (var process = Process.Start(startInfo))
                {
                    Assert.IsNotNull(process, $"Unable to start '{CapnpCompilation.CapnpCompilerFilename}'");

                    process.WaitForExit();

                    return process.ExitCode == 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        [Given(@"I have a binary code generator request ""(.*)""")]
        [Given(@"I have a binary code generator request (.*)")]
        public void GivenIHaveABinaryCodeGeneratorRequest(string binaryRequestFileName)
        {
            _inputStream = LoadResource(binaryRequestFileName);
            _nullableGenEnable = false; // Assume false by default, may be enabled later
        }

        [Given(@"my reference output is ""(.*)""")]
        public void GivenMyReferenceOutputIs(string expectedOutputFileName)
        {
            using (var stream = LoadResource(expectedOutputFileName))
            using (var reader = new StreamReader(stream))
            {
                _referenceOutputContent = reader.ReadToEnd();
            }
        }

        [When(@"I invoke capnpc-csharp")]
        public void WhenIInvokeCapnpc_Csharp()
        {
            Console.WriteLine($"Generate nullable reference types? {_nullableGenEnable}");

            using (_inputStream)
            {
                _result = CapnpCompilation.GenerateFromStream(_inputStream, new CodeGen.GeneratorOptions()
                {
                    NullableEnableDefault = _nullableGenEnable
                });
            }
        }
        
        [Then(@"the generated output must match the reference")]
        public void ThenTheGeneratedOutputMustMatchTheReference()
        {
            Assert.IsTrue(_result.IsSuccess, $"Tool invocation failed: {_result.Exception?.Message}");
            string generated = _result.GeneratedFiles.Single().GeneratedContent;
            bool equals = _referenceOutputContent.Equals(generated);
            if (!equals)
            {
                string path = Path.ChangeExtension(Path.GetTempFileName(), ".capnp.cs");
                File.WriteAllText(path, generated);

                string[] refLines = _referenceOutputContent.Split(Environment.NewLine);
                string[] actLines = generated.Split(Environment.NewLine);
                int mismatchLine = 0;

                for (int i = 0; i < Math.Min(refLines.Length, actLines.Length); i++)
                {
                    if (!refLines[i].Equals(actLines[i]))
                    {
                        mismatchLine = i + 1;
                        break;
                    }
                }

                Assert.Fail(
                    $"Reference output does not match. Expected: <{_referenceOutputContent.Substring(0, 100)}...>, actual: <{generated.Substring(0, 100)}...>, see {path}, first mismatch line: {mismatchLine}");
            }
        }

        [Then(@"the invocation must fail")]
        public void ThenTheInvocationMustFail()
        {
            Assert.IsFalse(_result.IsSuccess, "Tool invocation was supposed to fail, but it didn't");
            Assert.IsNotNull(_result.Exception, "Expected an exception");
        }

        [Given(@"capnp\.exe is installed on my system")]
        public void GivenCapnp_ExeIsInstalledOnMySystem()
        {
            if (!IsCapnpInstalled())
            {
                Assert.Inconclusive("Capnp compiler not found. Precondition of this test is not met.");
            }
        }

        [Given(@"I have a schema ""(.*)""")]
        public void GivenIHaveASchema(string capnpFileName)
        {
            _inputSchemaFileName = capnpFileName;

            using (var stream = LoadResource(capnpFileName))
            using (var reader = new StreamReader(stream))
            {
                _inputSchema = reader.ReadToEnd();
            }
        }

        [When(@"I try to generate code from that schema")]
        public void WhenIWantToGenerateCodeFromThatSchema()
        {
            string path = Path.Combine(Path.GetTempPath(), _inputSchemaFileName);
            File.WriteAllText(path, _inputSchema);
            _result = CapnpCompilation.InvokeCapnpAndGenerate(new string[] { path });
        }

        [Then(@"code generation must succeed")]
        public void ThenCodeGenerationMustSucceed()
        {
            Assert.IsNotNull(_result, "expected generation result");
            Assert.IsTrue(_result.IsSuccess, $"Tool invocation failed: {_result.Exception?.Message}");
            Assert.IsTrue(_result.GeneratedFiles.Count == 1, "Expected exactly one file");
            Assert.IsTrue(_result.GeneratedFiles[0].IsSuccess, $"Code generation failed: {_result.GeneratedFiles[0].Exception?.Message}");
            Assert.IsFalse(string.IsNullOrEmpty(_result.GeneratedFiles[0].GeneratedContent), "Expected non-empty generated content");
        }

        [Given(@"capnp\.exe is not installed on my system")]
        public void GivenCapnp_ExeIsNotInstalledOnMySystem()
        {
            if (IsCapnpInstalled())
            {
                Assert.Inconclusive("Capnp compiler found. Precondition of this test is not met.");
            }
        }

        [Then(@"the reason must be bad input")]
        public void ThenTheReasonMustBeBadInput()
        {
            Assert.IsTrue(_result.ErrorCategory == CapnpProcessFailure.BadInput);
        }

        [Then(@"the error output must contain ""(.*)""")]
        public void ThenTheErrorOutputMustContain(string p0)
        {
            Assert.IsTrue(_result.Messages.Any(m => m.FullMessage.Contains(p0)));
        }

        [Then(@"the error output must contain multiple messages")]
        public void ThenTheErrorOutputMustContainMultipleMessages()
        {
            Assert.IsTrue(_result.Messages.Count >= 2);
        }

        [Given(@"I enable generation of nullable reference types according to (.*)")]
        public void GivenIEnableGenerationOfNullableReferenceTypesAccordingTo(bool enable)
        {
            _nullableGenEnable = enable;
        }

        [Given(@"I enable the compiler support of nullable reference types according to (.*)")]
        public void GivenIEnableTheCompilerSupportOfNullableReferenceTypesAccordingTo(bool enable)
        {
            _nullableSupportEnable = enable;
        }

        [Then(@"the invocation must succeed and attempting to compile the generated code gives (.*)")]
        public void ThenTheInvocationMustSucceedAndAttemptingToCompileTheGeneratedCodeGives(string result)
        {
            Console.WriteLine($"Compiler supports nullable reference types? {_nullableSupportEnable}");

            Assert.IsTrue(_result.IsSuccess, "Tool invocation was not successful");
            var summary = Util.InlineAssemblyCompiler.TryCompileCapnp(
                _nullableSupportEnable ? NullableContextOptions.Enable : NullableContextOptions.Disable,
                _result.GeneratedFiles[0].GeneratedContent);

            try
            {
                switch (result)
                {
                    case "success":
                        Assert.AreEqual(
                            Util.InlineAssemblyCompiler.CompileSummary.Success,
                            summary,
                            "Compilation was expected to succeed");
                        break;

                    case "warnings":
                        Assert.AreEqual(
                            Util.InlineAssemblyCompiler.CompileSummary.SuccessWithWarnings,
                            summary,
                            "Compilation was expected to produce warnings");
                        break;

                    case "errors":
                        Assert.AreEqual(
                            Util.InlineAssemblyCompiler.CompileSummary.Error,
                            summary,
                            "Compilation was expected to fail");
                        break;

                    default:
                        Assert.Fail("Test case bug: unknown outcome specified");
                        break;

                }
            }
            catch (AssertFailedException)
            {
                string generated = _result.GeneratedFiles.Single().GeneratedContent;
                string path = Path.ChangeExtension(Path.GetTempFileName(), ".capnp.cs");
                File.WriteAllText(path, generated);
                Console.WriteLine($"Generated code was saved to {path}");

                throw;
            }
        }
    }
}
