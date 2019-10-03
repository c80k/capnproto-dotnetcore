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

        GenerationResult _result;

        public static Stream LoadResource(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string[] names = assembly.GetManifestResourceNames();
            string urn = Array.Find(names, n => n.EndsWith(name, StringComparison.OrdinalIgnoreCase));
            Assert.IsNotNull(urn, $"Test specification error: {name} does not exist");
            return assembly.GetManifestResourceStream(urn);
        }

        internal static bool IsCapnpExeInstalled()
        {
            using (var process = Process.Start("where", "capnp.exe"))
            {
                if (process == null)
                    Assert.Fail("Unable to start 'where'");

                process.WaitForExit();

                return process.ExitCode == 0;
            }
        }

        [Given(@"I have a binary code generator request ""(.*)""")]
        [Given(@"I have a binary code generator request (.*)")]
        public void GivenIHaveABinaryCodeGeneratorRequest(string binaryRequestFileName)
        {
            _inputStream = LoadResource(binaryRequestFileName);
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
            using (_inputStream)
            {
                _result = CapnpCompilation.GenerateFromStream(_inputStream);
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
            if (!IsCapnpExeInstalled())
            {
                Assert.Inconclusive("capnp.exe not found. Precondition of this test is not met.");
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
            if (IsCapnpExeInstalled())
            {
                Assert.Inconclusive("capnp.exe found. Precondition of this test is not met.");
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
    }
}
