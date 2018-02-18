using Nuke.Common.Tools.DotNet;
using Nuke.Core;
using System.IO;
using  Nuke.Common.Tools.Nunit;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Core.IO.PathConstruction;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.NuGet;
using Nuke.Core.Tooling;

namespace Build
{
    public partial class Build : NukeBuild
    {

        // Temp, should get these from nuget package source
        private string OpenCoverPath = "C:/test-tools/opencover/OpenCover.Console.exe";
        private string CoberturaConverterPath = "C:/test-tools/OpenCoverToCoberturaConverter/OpenCoverToCoberturaConverter.exe";

        [Parameter("The file to write the build version to (" + nameof(Export_Build_Version) + " target only)")] public string BuildVersionFilePath;
        public Target Export_Build_Version => _ => _
            .Description("Outputs the build version to a file")
            .Requires(() => BuildVersionFilePath)
            .Executes(() => File.WriteAllText(BuildVersionFilePath, GetBuildVersion()));



        public Target Compile => _ => _
          //  .DependsOn(Export_Build_Version)
            .Executes(() =>
                DotNetBuild(
                    RootDirectory,
                    settings => settings
                    .SetConfiguration("Release")
                        .SetFileVersion(GetSemanticBuildVersion())
                        .SetInformationalVersion(GetBuildVersion())
                        .AddProperty("Version", GetBuildVersion())));

        public Target UnitTest => _ => _
            .DependsOn(Compile)
            .Executes(() => 
             DotNetTest(settings => settings
                            .SetProjectFile(RootDirectory / "CrownBet.QA.Common.Tests")
							.SetConfiguration("Release")
                            .SetLogger("xunit;LogFilePath=TestResults.xml")
                         .SetNoBuild(true))
            );

        public Target CodeCoverage => _ => _
       .Description("Perform all unit tests")
       .DependsOn(Compile)
       .Executes(() =>
       {
           string[] testProjects = { "CrownBet.QA.Common.Tests"};
           foreach (var testProject in testProjects)
           {
               string opencoverParams = $"-register:user " +
               $"-target:dotnet.exe " +
               $"\"-targetdir:{ testProject}\" " +
               $"\"-targetargs:test\" " +
               $"\"-output:{RootDirectory}/OpenCover.coverageresults\" " +
               $"-mergeoutput " +
               $"-oldStyle " +
               $"-excludebyattribute:System.CodeDom.Compiler.GeneratedCodeAttribute " +
               $"\"-filter:+[CrownBet.QA.Common]* -[*Tests]* -[Moq]* -[*]Moq*\"";

               ProcessTasks.StartProcess(
                            OpenCoverPath, opencoverParams, RootDirectory).AssertZeroExitCode();
          }
           string coberturaParams = $"-input:\"{RootDirectory}/OpenCover.coverageresults\" -output:\"{RootDirectory}/Cobertura.coverageresults\" -sources:\"{RootDirectory}\"";
           ProcessTasks.StartProcess(
                        CoberturaConverterPath, coberturaParams, RootDirectory).AssertZeroExitCode();
       });


        private string LibraryName => Path.GetFileNameWithoutExtension(SolutionFile);

        private AbsolutePath LibrarySourceDirectory => RootDirectory / LibraryName;

        private GlobalSettings Settings =>  new GlobalSettings();

        public static int Main() => Execute<Build>(x => x.Compile);

        private string GetBuildVersion()
        {
            // string branch = Git.GetBranchName(RootDirectory);
            string branch = "master";

            branch = branch == "master" ? "" : "-" + branch.Replace("/", "-");

            return GetSemanticBuildVersion() + branch;
        }

        private string GetSemanticBuildVersion()
        {
            //   return $"1.0.{Git.GetCommitCount(RootDirectory)}";
           return $"1.0.{1}";
        }
    }
}