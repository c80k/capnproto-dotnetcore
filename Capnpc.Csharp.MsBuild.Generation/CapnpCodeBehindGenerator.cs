using System;
using System.Collections.Generic;
using System.IO;

namespace Capnpc.Csharp.MsBuild.Generation
{
    public class CapnpCodeBehindGenerator : IDisposable
    {
        //private SpecFlowProject _specFlowProject;
        //private ITestGenerator _testGenerator;

        public void InitializeProject(string projectPath)
        {
            //_specFlowProject = MsBuildProjectReader.LoadSpecFlowProjectFromMsBuild(Path.GetFullPath(projectPath), rootNamespace);

            //var projectSettings = _specFlowProject.ProjectSettings;

            //var testGeneratorFactory = new TestGeneratorFactory();

            //_testGenerator = testGeneratorFactory.CreateGenerator(projectSettings, generatorPlugins);
        }


        public TestFileGeneratorResult GenerateCodeBehindFile(string capnpFile)
        {
            //var featureFileInput = new FeatureFileInput(featureFile);
            //var generatedFeatureFileName = Path.GetFileName(_testGenerator.GetTestFullPath(featureFileInput));

            //var testGeneratorResult = _testGenerator.GenerateTestFile(featureFileInput, new GenerationSettings());

            return new TestFileGeneratorResult(
                new TestGeneratorResult() { GeneratedTestCode = "//dummy" }, 
                capnpFile + ".cs");
        }

        public void Dispose()
        {
            //_testGenerator?.Dispose();
        }
    }
}