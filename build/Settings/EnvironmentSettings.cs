using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Amazon.CloudFormation.Model;
using Nuke.Core.Utilities.Collections;

namespace Build.Settings
{
    public abstract class EnvironmentSettings
    {
        protected readonly string EnvironmentName;
        private readonly string _accountNumber;

        protected EnvironmentSettings(string accountNumber, string environmentName)
        {
            _accountNumber = accountNumber;
            EnvironmentName = environmentName;

            Application = new ApplicationSettings();
        }

        public ApplicationSettings Application { get; }


        public string DeployRoleArn => $"arn:aws:iam::{_accountNumber}:role/DinkumCoin-{EnvironmentName}-DeployRole";  


        public string AmiId { get; set; }
        public string AlbSecurityGroupIds { get; set; }
        public string HostedZoneName { get; set; }

        public string InstanceType { get; set; }
        public string Ec2SecurityGroupIds { get; set; }


        public int MaxInstanceCount { get; set; }

        public int MinInstanceCount { get; set; }

        public int DesiredInstanceCount { get; set; }

        public string SubnetIds { get; set; }
        public string VpcCidr { get; set; }

        public string VpcId { get; set; }

        public string LogLambdaArn { get; set; } 






        public static EnvironmentSettings CreateSettings(string accountName, string environmentName)
        {
            Type[] types = typeof(EnvironmentSettings)
                .GetTypeInfo()
                .Assembly.GetTypes()
                .Where(type => type.IsSubclassOf(typeof(EnvironmentSettings)))
                .ToArray();


            Console.WriteLine($"Number of setting files found: {types.Length}");
            types.ForEach(x => Console.WriteLine(x.Name));

            Type settingType =
                types.FirstOrDefault(type => string.Equals(type.Name, accountName + environmentName, StringComparison.OrdinalIgnoreCase)) ??
                types.FirstOrDefault(type => string.Equals(type.Name, accountName, StringComparison.OrdinalIgnoreCase));

            if (settingType == null)
            {
                throw new ArgumentOutOfRangeException($"Unable to find environment settings for {environmentName} environment or {accountName} account");
            }

            return (EnvironmentSettings)Activator.CreateInstance(settingType, environmentName);
        }

        public static List<Tag> CreateStackTags(string environmentName)
        {
            var tags = new List<Tag>
            {
                new Tag { Key = "Platform", Value = GlobalSettings.ApplicationName },
                new Tag { Key = "Department", Value = "QA" }
            };

            if (environmentName != null)
            {
                tags.Add(new Tag { Key = "environment", Value = environmentName.ToUpper() });
            }

            return tags;
        }

        public class ApplicationSettings
        {

            public string KeyPairName { get; set; }

        }

  
    }
}