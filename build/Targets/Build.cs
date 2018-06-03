using Build.Settings;
using Build;
using Build.Aws;
using Nuke.Common.Tools.DotNet;
using Nuke.Core;
using Nuke.Core.IO;
using Nuke.Core.Tooling;
using System.IO;
using System.IO.Compression;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Amazon.SecurityToken.Model;

namespace Build.Targets
{
    public partial class Build : NukeBuild
    {
        [Parameter("The file to write the build version to (" + nameof(Export_Build_Version) + " target only)")] public string BuildVersionFilePath;

        private string _buildVersion;
        private GlobalSettings Settings => _globalSettings = _globalSettings ?? new GlobalSettings(RootDirectory);

        private GlobalSettings _globalSettings;

        public Target Export_Build_Version => _ => _
             .Description("Outputs the build version to a file")
             .Requires(() => BuildVersionFilePath)
             .Executes(() => File.WriteAllText(BuildVersionFilePath, GetBuildVersion()));
        
        public Target Clean => _ => _
             .Description("Remove previous build output")
             .Executes(() => FileSystemTasks.DeleteDirectory(Settings.BuildOutputDirectory));

        public Target Compile => _ => _
             .DependsOn(Clean)
             .Description("Build all projects in the solution")
            .Executes(() => DotNetBuild(settings => settings
                                        .SetConfiguration("Release")
                                        .SetProjectFile(SolutionDirectory)
                                       ));

        public Target Unit_Test => _ => _
             .Description("Perform all unit tests")
             .DependsOn(Compile)
             .Executes(() =>
            {
            DotNetTest(
                settings => settings
                    .SetProjectFile(Settings.TestDirectory / "DinkumCoin.Data.Tests")
                .SetConfiguration("Release")
                    .SetLogger("xunit;LogFilePath=TestResults.xml")
                    .SetNoBuild(true));
            
            DotNetTest(
        settings => settings
            .SetProjectFile(Settings.TestDirectory / "DinkumCoin.Services.Tests")
                .SetConfiguration("Release")
            .SetLogger("xunit;LogFilePath=TestResults.xml")
            .SetNoBuild(true));

            });

        public Target Verify_Pacts => _ => _
             .Description("Verify pacts")
             .DependsOn(Compile)
             .Executes(() => DotNetTest(
                 settings => settings
                .SetConfiguration("Release")
                .SetProjectFile(Settings.TestDirectory / "DinkumCoin.Api.PactVerify")
                .SetNoBuild(true))
            );

        public Target Upload => _ => _
             .Description("Upload application package to S3")
             .DependsOn(Package)
             .Executes(() =>
             {
                 Credentials credentials = Sts.AssumeRole(GlobalSettings.BucketWriteRoleArn, "ci-build").Result;
                 S3.UploadDirectory(Settings.PackageDirectory, GlobalSettings.BucketName, GetBuildVersion(), credentials: credentials).Wait();
                 S3.UploadDirectory(Settings.Ec2ScriptsDirectory, GlobalSettings.BucketName, $"{GetBuildVersion()}/ec2", true, credentials).Wait();

             });

        public Target Package => _ => _
             .Description("Package the application")
            .DependsOn(Unit_Test)
             .Executes(() =>
             {
                ProcessTasks.StartProcess(
                DotnetPath, $"publish -c Release -r linux-x64 /p:Version=\"{GetBuildVersion()}\" -o \"{Settings.PublishDirectory}\"", Settings.SourceDirectory / "DinkumCoin.Api").AssertZeroExitCode();

                 Directory.CreateDirectory(Settings.PackageDirectory);
                 ZipFile.CreateFromDirectory(Settings.PublishDirectory, Settings.PackageDirectory / $"DinkumCoin.Api_{GetBuildVersion()}.zip");
             });

        private string DotnetPath { get; } = new DotNetSettings().ToolPath;


        public static int Main() => Execute<Build>(x => x.Package);

        private string GetBuildVersion()
        {
            if (_buildVersion != null) { return _buildVersion; }

            string branch = Git.GetBranchName(RootDirectory);

            branch = branch == "master" ? "" : "-" + branch.Replace("/", "-").Substring(0,14);

            return _buildVersion = GetSemanticBuildVersion() + branch;
        }

        private string GetSemanticBuildVersion()
        {
            return $"1.0.{Git.GetCommitCount(RootDirectory)}";
        }
    }
}
