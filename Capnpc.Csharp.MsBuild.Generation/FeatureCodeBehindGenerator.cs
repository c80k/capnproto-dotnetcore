﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Capnpc.Csharp.MsBuild.Generation
{
    public class FeatureCodeBehindGenerator : IDisposable
    {
        //private SpecFlowProject _specFlowProject;
        //private ITestGenerator _testGenerator;

        public void InitializeProject(string projectPath, string rootNamespace, IEnumerable<string> generatorPlugins)
        {
            //_specFlowProject = MsBuildProjectReader.LoadSpecFlowProjectFromMsBuild(Path.GetFullPath(projectPath), rootNamespace);

            //var projectSettings = _specFlowProject.ProjectSettings;

            //var testGeneratorFactory = new TestGeneratorFactory();

            //_testGenerator = testGeneratorFactory.CreateGenerator(projectSettings, generatorPlugins);
        }


        public TestFileGeneratorResult GenerateCodeBehindFile(string featureFile)
        {
            //var featureFileInput = new FeatureFileInput(featureFile);
            //var generatedFeatureFileName = Path.GetFileName(_testGenerator.GetTestFullPath(featureFileInput));

            //var testGeneratorResult = _testGenerator.GenerateTestFile(featureFileInput, new GenerationSettings());

            return new TestFileGeneratorResult(null, null);
        }

        public void Dispose()
        {
            //_testGenerator?.Dispose();
        }
    }
}