namespace Build.Settings
{
    public class Wgtpoc : EnvironmentSettings
    {
        public Wgtpoc(string environmentName) : base(GlobalSettings.WgtpocAccountId, environmentName)
        {
            HostedZoneName = "poc.cbenv.com.au";

            AlbSecurityGroupIds = "sg-15439b72";
            Ec2SecurityGroupIds = "sg-15439b72,sg-0b9b1872";
            Application.KeyPairName = "wgtpoc";

            InstanceType = "t2.nano";
            DesiredInstanceCount = 1;
            MinInstanceCount = 1;
            MaxInstanceCount = 1;
            SubnetIds = "subnet-d4a4d5b0,subnet-07ac3771,subnet-e256ebbb";
            VpcCidr = "10.126.0.0/16";
            VpcId = "vpc-17c41d73";
            LogLambdaArn = "arn:aws:lambda:ap-southeast-2:250658028269:function:CloudwatchLogsSumoTransferFunction-poc";
        }
    }
}