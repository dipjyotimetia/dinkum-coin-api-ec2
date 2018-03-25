namespace Build.Settings
{
    public class Wgtpoc : EnvironmentSettings
    {
        public Wgtpoc(string environmentName) : base(GlobalSettings.WgtpocAccountId, environmentName)
        {
            HostedZoneName = "poc.cbenv.com.au";

            //AlbSecurityGroupIds = "sg-15439b72";
            AlbSecurityGroupIds = "sg-0794177e";
            //Ec2SecurityGroupIds = "sg-15439b72,sg-71b20a08,sg-86ab13ff";
            Ec2SecurityGroupIds = "sg-0794177e";
            Application.KeyPairName = "wgtpoc";

            InstanceType = "t2.nano";
            DesiredInstanceCount = 1;
            MinInstanceCount = 1;
            MaxInstanceCount = 2;
            //SubnetIds = "subnet-0a90f56e, subnet-cb46cabd, subnet-e2b41abb";
            SubnetIds = "subnet-80d7c0d9,subnet-5bd6503c";
            VpcCidr = "10.126.0.0/16";
            //VpcId = "vpc-17c41d73";
            VpcId = "vpc-2b778b4c";
            LogLambdaArn = "arn:aws:lambda:ap-southeast-2:250658028269:function:CloudwatchLogsSumoTransferFunction-poc";
        }
    }
}