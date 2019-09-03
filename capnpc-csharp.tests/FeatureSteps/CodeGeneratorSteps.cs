using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Reflection;
using TechTalk.SpecFlow;

namespace capnpc_csharp.Tests
{
    [Binding]
    public class CodeGeneratorSteps
    {
        Stream _inputStream;
        string _referenceOutputContent;
        string _exceptedOutputFileName;
        string _actualGeneratedContent;
        bool _success;
        Exception _generateException;

        internal static Stream LoadResource(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string[] names = assembly.GetManifestResourceNames();
            string urn = Array.Find(names, n => n.EndsWith(name, StringComparison.OrdinalIgnoreCase));
            Assert.IsNotNull(urn, $"Test specification error: {name} does not exist");
            return assembly.GetManifestResourceStream(urn);
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
            _exceptedOutputFileName = expectedOutputFileName;
            using (var stream = LoadResource(expectedOutputFileName))
            using (var reader = new StreamReader(stream))
            {
                _referenceOutputContent = reader.ReadToEnd();
            }
        }

        [When(@"I invoke capnpc-csharp")]
        public void WhenIInvokeCapnpc_Csharp()
        {
            try
            {
                using (_inputStream)
                {
                    string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                    Directory.CreateDirectory(tempDir);
                    Environment.CurrentDirectory = tempDir;

                    CapnpC.Program.GenerateFromStream(_inputStream);

                    string outPath = Path.Combine(tempDir, _exceptedOutputFileName);
                    _actualGeneratedContent = File.ReadAllText(outPath);
                    _success = true;
                }
            }
            catch (Exception exception)
            {
                _generateException = exception;
            }
        }
        
        [Then(@"the generated output must match the reference")]
        public void ThenTheGeneratedOutputMustMatchTheReference()
        {
            Assert.IsTrue(_success, $"Code generation failed: {_generateException?.Message}");
            Assert.AreEqual(_referenceOutputContent, _actualGeneratedContent);
        }

        [Then(@"the invocation must fail")]
        public void ThenTheInvocationMustFail()
        {
            Assert.IsFalse(_success, "Code generation was supposed to fail, but it didn't");
        }
    }
}
