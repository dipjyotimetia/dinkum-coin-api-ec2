using Amazon.SecurityToken.Model;
using Build.Settings;
using Build.Aws;
using Nuke.Common.Tools.DotNet;
using Nuke.Core;
using Nuke.Core.Tooling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using Parameter = Amazon.CloudFormation.Model.Parameter;
using Tag = Amazon.CloudFormation.Model.Tag;
using Target = Nuke.Core.Target;


namespace Build.Targets
{
    public partial class Build
    {
        [Parameter("Name of AWS account (Deploy targets only)", Name = "Account")] public string AccountName;
        [Parameter("Name of environment (Deploy targets only)", Name = "Environment")] public string EnvironmentName;
        [Parameter("Version of application to deploy (Deploy targets only)")] public string VersionToDeploy;
       
        private Credentials _credentials;
        private EnvironmentSettings _environmentSettings;
        private StackName _stackName;
        private List<Tag> _stackTags;

        private string GatlingPath { get; } = new GatlingSettings().ToolPath;

        public Target Deploy => _ => _
            .Description("Provision AWS resources for the application with accompanying dashboards and alerts")
            .DependsOn(Deploy_Application);

        public Target Deploy_Application => _ => _
            .Description("Provision AWS resources for the application")
            .Requires(() => AccountName)
            .Requires(() => EnvironmentName)
            .Executes(() =>
            {

            UpsertStack(
                StackName.Application, Settings.TemplateDirectory / "Application.yaml",
                new List<Parameter>
                {
                    new Parameter { ParameterKey = "AmiId", ParameterValue = GlobalSettings.AmiId },
                    new Parameter { ParameterKey = "AlbSecurityGroupIds", ParameterValue = EnvironmentSettings.AlbSecurityGroupIds },
                    new Parameter { ParameterKey = "HostedZoneName", ParameterValue = EnvironmentSettings.HostedZoneName },
                    new Parameter { ParameterKey = "InstanceType", ParameterValue = EnvironmentSettings.InstanceType },
                    new Parameter { ParameterKey = "Ec2SecurityGroupIds",ParameterValue = EnvironmentSettings.Ec2SecurityGroupIds},
                    new Parameter { ParameterKey = "EnvironmentName", ParameterValue = EnvironmentName },
                    new Parameter { ParameterKey = "EnvironmentShortName", ParameterValue = EnvironmentName.ToLower() },
                    new Parameter {ParameterKey = "DesiredInstanceCount", ParameterValue = EnvironmentSettings.DesiredInstanceCount.ToString()},
                    new Parameter {ParameterKey = "MaxInstanceCount", ParameterValue = EnvironmentSettings.MaxInstanceCount.ToString()},
                    new Parameter {ParameterKey = "MinInstanceCount", ParameterValue = EnvironmentSettings.MinInstanceCount.ToString()},
                    new Parameter { ParameterKey = "KeyPairName", ParameterValue = EnvironmentSettings.Application.KeyPairName },
                    new Parameter { ParameterKey = "LogLambdaArn", ParameterValue = EnvironmentSettings.LogLambdaArn },
                    new Parameter { ParameterKey = "S3BucketName", ParameterValue = GlobalSettings.BucketName },
                    new Parameter { ParameterKey = "SubnetIds", ParameterValue = EnvironmentSettings.SubnetIds },
                    new Parameter { ParameterKey = "VersionToDeploy", ParameterValue = VersionToDeploy, UsePreviousValue = (VersionToDeploy == null) },
                new Parameter { ParameterKey = "VpcCidr", ParameterValue = EnvironmentSettings.VpcCidr },
                    new Parameter { ParameterKey = "VpcId", ParameterValue = EnvironmentSettings.VpcId },
                }, StackTags.Union(new[] { new Tag { Key = "Version", Value = GetBuildVersion() } }).ToList(), true);


            var appRoleName =  GetStackOutputValue(StackName.Application, "DinkumApiAppRoleName");

            UpsertStack(
                StackName.DynamoDB, Settings.TemplateDirectory / "DynamoDB.yaml",
                new List<Parameter>
                {
                new Parameter { ParameterKey = "DinkumApiAppIamRole", ParameterValue = appRoleName }
                }, StackTags, true);
            });



        public Target Integration_Test => _ => _
          .Description("Post-deployment integration tests")
          .Requires(() => AccountName)
          .Requires(() => EnvironmentName)
          .Executes(() =>
          {

          });


        public Target Deploy_S3 => _ => _
            .Description("Provision S3 resources")
            .Executes(() =>
            {
                if (Sts.GetCallerAccount().Result != GlobalSettings.WgtdevAccountId)
                {
                    throw new InvalidOperationException("S3 resources can only be deployed to the wgtdev account. Security credentials for the wgtdev account are required. Refer to readme.md for more details.");
                }

                CloudFormation.UpsertStack(
                    StackName.S3, Settings.TemplateDirectory / "S3.yaml",
                    new List<Parameter>
                    {
                        new Parameter { ParameterKey = "S3BucketName", ParameterValue = GlobalSettings.BucketName },
                new Parameter { ParameterKey = "AllowedRoleArns", ParameterValue = GlobalSettings.JenkinsRoleArn }
                    }, StackTags, Credentials).Wait();
            });

    

        private Credentials Credentials => _credentials = _credentials ?? Sts.AssumeRole(EnvironmentSettings.DeployRoleArn, "deploy").Result;

        private EnvironmentSettings EnvironmentSettings => _environmentSettings = _environmentSettings ?? EnvironmentSettings.CreateSettings(AccountName, EnvironmentName);


        private StackName StackName => _stackName = _stackName ?? new StackName(EnvironmentName);

        private List<Tag> StackTags => _stackTags = _stackTags ?? EnvironmentSettings.CreateStackTags(EnvironmentName);

        private IDictionary<string, string> GetEnvironmentVariables()
        {
            IDictionary environmentVariables = Environment.GetEnvironmentVariables();

            return environmentVariables
                .Keys
                .Cast<string>()
                .ToDictionary(key => key, key => (string)environmentVariables[key]);
        }

        private string GetStackOutputValue(string stackName, string key) => CloudFormation.GetStackOutputValue(stackName, key, Credentials).Result;

        private void UpsertStack(string stackName, string templatePath, List<Parameter> parameters, List<Tag> tags, bool notification = false)
        {
            var settings = new StackSettings
            {
                NotificationArns = new List<string>()
            };

            CloudFormation.UpsertStack(stackName, templatePath, parameters, tags, Credentials, settings).Wait();
        }
    }
}
