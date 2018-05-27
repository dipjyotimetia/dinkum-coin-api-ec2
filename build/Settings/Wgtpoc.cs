namespace Build.Settings
{
    public class Wgtpoc : EnvironmentSettings
    {
        public Wgtpoc(string environmentName) : base(GlobalSettings.WgtpocAccountId, environmentName)
        {
            HostedZoneName = "poc.cbenv.com.au";

            AlbSecurityGroupIds = "sg-15439b72";
            Ec2SecurityGroupIds = "sg-15439b72,sg-0b9b1872";
            Application.KeyPairName = "Stu-P-key";

            InstanceType = "t2.nano";
            DesiredInstanceCount = 1;
            MinInstanceCount = 1;
            MaxInstanceCount = 2;
            SubnetIds = "subnet-ffb41aa6,subnet-0c46ca7a,subnet-dd91f4b9";
           // VpcCidr = "10.126.0.0/16";
            VpcCidr = "0.0.0.0/0";
            VpcId = "vpc-17c41d73";
            LogLambdaArn = "arn:aws:lambda:ap-southeast-2:250658028269:function:CloudwatchLogsSumoTransferFunction-poc";
        
        }
    }
}