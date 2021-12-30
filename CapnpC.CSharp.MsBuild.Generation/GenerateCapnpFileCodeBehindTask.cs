using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace CapnpC.CSharp.MsBuild.Generation
{
    public class GenerateCapnpFileCodeBehindTask : Task
    {
        public GenerateCapnpFileCodeBehindTask()
        {
            CodeBehindGenerator = new CapnpFileCodeBehindGenerator(Log);
        }

        public ICapnpcCsharpGenerator CodeBehindGenerator { get; set; }

        [Required]
        public string ProjectPath { get; set; }

        public string ProjectFolder => Path.GetDirectoryName(ProjectPath);

        public ITaskItem[] CapnpFiles { get; set; }

        [Output]
        public ITaskItem[] GeneratedFiles { get; private set; }

        static CapnpGenJob ToGenJob(ITaskItem item)
        {
            var job = new CapnpGenJob()
            {
                CapnpPath = item.GetMetadata("FullPath"),
                WorkingDirectory = item.GetMetadata("WorkingDirectory")
            };

            string importPaths = item.GetMetadata("ImportPaths");

            if (!string.IsNullOrWhiteSpace(importPaths))
            {
                job.AdditionalArguments.AddRange(importPaths.Split(new char[] { ';' }, 
                    StringSplitOptions.RemoveEmptyEntries).Select(p => $"-I{p}"));
            }

            string sourcePrefix = item.GetMetadata("SourcePrefix");

            if (!string.IsNullOrWhiteSpace(sourcePrefix))
            {
                job.AdditionalArguments.Add(sourcePrefix);
            }


            string verbose = item.GetMetadata("Verbose");

            if ("true".Equals(verbose, StringComparison.OrdinalIgnoreCase))
            {
                job.AdditionalArguments.Add("--verbose");
            }

            return job;
        }

        public override bool Execute()
        {
            try
            {
                try
                {
                    var currentProcess = Process.GetCurrentProcess();

                    Log.LogWithNameTag(Log.LogMessage, $"process: {currentProcess.ProcessName}, pid: {currentProcess.Id}, CD: {Environment.CurrentDirectory}");

                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        Log.LogWithNameTag(Log.LogMessage, "  " + assembly.FullName);
                    }
                }
                catch (Exception e)
                {
                    Log.LogWithNameTag(Log.LogMessage, $"Error when dumping process info: {e}");
                }

                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

                var generator = CodeBehindGenerator ?? new CapnpFileCodeBehindGenerator(Log);

                Log.LogWithNameTag(Log.LogMessage, "Starting GenerateCapnpFileCodeBehind");

                var capnpFiles = CapnpFiles?.Select(ToGenJob).ToList() ?? new List<CapnpGenJob>();

                var generatedFiles = generator.GenerateFilesForProject(
                    ProjectPath,
                    capnpFiles,
                    ProjectFolder);

                GeneratedFiles = generatedFiles.Select(file => new TaskItem { ItemSpec = file }).ToArray();

                return !Log.HasLoggedErrors;
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    if (e.InnerException is FileLoadException fle)
                    {
                        Log?.LogWithNameTag(Log.LogError, $"FileLoadException Filename: {fle.FileName}");
                        Log?.LogWithNameTag(Log.LogError, $"FileLoadException FusionLog: {fle.FusionLog}");
                        Log?.LogWithNameTag(Log.LogError, $"FileLoadException Message: {fle.Message}");
                    }

                    Log?.LogWithNameTag(Log.LogError, e.InnerException.ToString());
                }

                Log?.LogWithNameTag(Log.LogError, e.ToString());
                return false;
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
            }
        }

        private System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Log.LogWithNameTag(Log.LogMessage, args.Name);
            

            return null;
        }

    }


}