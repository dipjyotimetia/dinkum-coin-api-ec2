namespace Build.Settings
{
    public class StackName
    {
        public const string KmsKey = "KmsKey-QA";
        public string LogLambda;
        public const string S3 = "DinkumCoin-S3";


        private readonly string _environment;

        public StackName(string environment)
        {
            _environment = environment;
        }

        public string DynamoDB => $"{Prefix}-DynamoDB";

        public string Application => $"{Prefix}-Application";

        public string DeployRole => $"{Prefix}-DeployRole";

        private string Prefix => $"DinkumCoin-{_environment}";
    }
}