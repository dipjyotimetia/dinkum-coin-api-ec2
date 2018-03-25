using static Nuke.Core.IO.PathConstruction;

namespace Build.Settings
{
    public class GlobalSettings
    {
        public const string AmiId = "ami-ad5ba9cf";
        public const string ApplicationName = "Dinkum Coin API";
        public const string BucketName = "dinkum-coin-api-packages-dev";
        public const string BucketWriteRoleArn = "arn:aws:iam::" + WgtpocAccountId + ":role/DinkumCoinApi-WriteBucketRole";
        public const string JenkinsRoleArn = "arn:aws:iam::" + WgtpocAccountId + ":role/jenkins-ci";

        public const string WgtdevAccountId = "886153924892";
        public const string WgtpocAccountId = "250658028269";

        public GlobalSettings(AbsolutePath rootDirectory)
        {
            SourceDirectory = rootDirectory / "src";
            TestDirectory = rootDirectory / "test";
            TemplateDirectory = rootDirectory / "deploy";
            BuildOutputDirectory = rootDirectory / "buildOutput";
            LogsDirectory = BuildOutputDirectory / "logs";
            PackageDirectory = BuildOutputDirectory / "package";
            PublishDirectory = BuildOutputDirectory / "publish";
        }

        public AbsolutePath BuildOutputDirectory { get; }

        public AbsolutePath PackageDirectory { get; }

        public AbsolutePath PublishDirectory { get; }

        public AbsolutePath LogsDirectory { get; }

        public AbsolutePath SourceDirectory { get; }

        public AbsolutePath TemplateDirectory { get; }

        public AbsolutePath TestDirectory { get; }
    }
}