using Amazon.CloudFormation;
using Amazon.CloudFormation.Model;
using Amazon.SecurityToken.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Build.Aws
{
    public static class CloudFormation
    {
        public static async Task<string> CreateChangeset(
            string stackName, string templatePath, List<Parameter> parameters, ChangeSetType type, Credentials credentials)
        {
            Console.WriteLine($"\nCreating change set for {stackName}...");

            return (await GetClient(credentials).CreateChangeSetAsync(
                new CreateChangeSetRequest
                {
                    Capabilities = new List<string> { Capability.CAPABILITY_NAMED_IAM },
                    ChangeSetName = $"nuke-deploy-{DateTimeOffset.Now.ToUnixTimeSeconds()}",
                    ChangeSetType = type,
                    StackName = stackName,
                    TemplateBody = File.ReadAllText(templatePath),
                    Parameters = parameters
                })).Id;
        }

        public static async Task CreateStack(
            string stackName, string templatePath, List<Parameter> parameters, List<Tag> tags, Credentials credentials, StackSettings settings = null)
        {
            Console.WriteLine($"\nCreating stack {stackName}...");

            await GetClient(credentials).CreateStackAsync(
                new CreateStackRequest
                {
                    StackName = stackName,
                    TemplateBody = File.ReadAllText(templatePath),
                    Parameters = parameters,
                    Capabilities = new List<string> { Capability.CAPABILITY_NAMED_IAM },
                    Tags = tags,
                    DisableRollback = true,
                    NotificationARNs = settings?.NotificationArns?.ToList() ?? new List<string>()
                });

            await WaitForStackStatus(stackName, StackStatus.CREATE_COMPLETE, credentials);
        }

        public static async Task DeleteStack(string stackName, Credentials credentials)
        {
            Console.WriteLine($"\nDeleting stack {stackName}...");

            try
            {
                await GetClient(credentials).DescribeStacksAsync(new DescribeStacksRequest { StackName = stackName });
            }
            catch (AmazonCloudFormationException exc) when (exc.Message.Contains("does not exist"))
            {
                Console.WriteLine("Stack does not exist");
                return;
            }

            DescribeStacksResponse response = await GetClient(credentials).DescribeStacksAsync(new DescribeStacksRequest { StackName = stackName });
            Stack stack = response.Stacks.First();

            Console.WriteLine(stack.StackId);
            await GetClient(credentials).DeleteStackAsync(new DeleteStackRequest { StackName = stack.StackId });
            await WaitForStackStatus(stack.StackId, StackStatus.DELETE_COMPLETE, credentials);
        }

        public static async Task DeployStack(string stackName, string templatePath, List<Parameter> parameters, List<Tag> tags, Credentials credentials)
        {
            Console.WriteLine($"\nDeploying stack {stackName}...");

            bool stackExists = await StackExists(stackName, credentials);
            string changeSetArn = await CreateChangeset(
                stackName, templatePath, parameters, stackExists ? ChangeSetType.UPDATE : ChangeSetType.CREATE, credentials);

            try
            {
                await WaitForChangeSetStatus(changeSetArn, stackName, ChangeSetStatus.CREATE_COMPLETE, credentials);
            }
            catch (EmptyChangeException)
            {
                return;
            }

            await ExecuteChangeSet(changeSetArn, stackName, credentials);
            await WaitForStackStatus(stackName, stackExists ? StackStatus.UPDATE_COMPLETE : StackStatus.CREATE_COMPLETE, credentials);
        }

        public static async Task<string> GetPhysicalStackResourceId(string stackName, string logicalResourceId, Credentials credentials)
        {
            DescribeStackResourcesResponse response = await GetClient(credentials).DescribeStackResourcesAsync(
                new DescribeStackResourcesRequest { StackName = stackName });

            return response.StackResources.FirstOrDefault(resource => resource.LogicalResourceId == logicalResourceId)?.PhysicalResourceId;
        }

        public static async Task<string> GetStackOutputValue(string stackName, string key, Credentials credentials, bool throwOnMissingStack = true)
        {
            DescribeStacksResponse response;

            try
            {
                response = await GetClient(credentials).DescribeStacksAsync(new DescribeStacksRequest { StackName = stackName });
            }
            catch (AmazonCloudFormationException exc) when (exc.Message.Contains("does not exist"))
            {
                if (throwOnMissingStack)
                {
                    throw;
                }

                return null;
            }

            return response.Stacks.First().Outputs.First(o => o.OutputKey == key).OutputValue;
        }

        public static async Task<bool> StackExists(string stackName, Credentials credentials)
        {
            try
            {
                await GetStackStatus(stackName, credentials);

                return true;
            }
            catch (AmazonCloudFormationException exc) when (exc.Message.Contains("does not exist")) { }

            return false;
        }

        public static async Task UpdateStack(
            string stackName, string templatePath, List<Parameter> parameters, List<Tag> tags, Credentials credentials, StackSettings settings = null)
        {
            Console.WriteLine($"\nUpdating stack {stackName}...");

            try
            {
                await GetClient(credentials).UpdateStackAsync(
                    new UpdateStackRequest
                    {
                        StackName = stackName,
                        TemplateBody = File.ReadAllText(templatePath),
                        Parameters = parameters,
                        Capabilities = new List<string> { Capability.CAPABILITY_NAMED_IAM },
                        Tags = tags,
                        NotificationARNs = settings?.NotificationArns?.ToList() ?? new List<string>()
                    });
            }
            catch (AmazonCloudFormationException exc) when (exc.Message == "No updates are to be performed.")
            {
                Console.WriteLine(exc.Message);

                return;
            }

            await WaitForStackStatus(stackName, StackStatus.UPDATE_COMPLETE, credentials);
        }

        public static async Task UpsertStack(
            string stackName, string templatePath, List<Parameter> parameters, List<Tag> tags, Credentials credentials, StackSettings settings = null)
        {
            if (await StackExists(stackName, credentials))
            {
                await UpdateStack(stackName, templatePath, parameters, tags, credentials, settings);
            }
            else
            {
                await CreateStack(stackName, templatePath, parameters, tags, credentials, settings);
            }
        }

        private static async Task<DescribeChangeSetResponse> DescribeChangeSetStatus(string changeSetId, string stackName, Credentials credentials)
        {
            return await GetClient(credentials).DescribeChangeSetAsync(
                new DescribeChangeSetRequest
                {
                    ChangeSetName = changeSetId,
                    StackName = stackName
                });
        }

        private static async Task ExecuteChangeSet(string changeSetId, string stackName, Credentials credentials)
        {
            Console.WriteLine($"\nExecuting change set for {stackName}...");

            await GetClient(credentials).ExecuteChangeSetAsync(
                new ExecuteChangeSetRequest
                {
                    ChangeSetName = changeSetId,
                    StackName = stackName
                });
        }

        private static AmazonCloudFormationClient GetClient(Credentials credentials = null)
        {
            return credentials == null ? new AmazonCloudFormationClient() : new AmazonCloudFormationClient(credentials);
        }

        private static async Task<StackStatus> GetStackStatus(string stackName, Credentials credentials)
        {
            return (await GetClient(credentials).DescribeStacksAsync(new DescribeStacksRequest { StackName = stackName })).Stacks.First().StackStatus;
        }

        private static async Task WaitForChangeSetStatus(string changeSetId, string stackName, ChangeSetStatus expectedStatus, Credentials credentials)
        {
            DescribeChangeSetResponse response = await DescribeChangeSetStatus(changeSetId, stackName, credentials);

            while (response.Status != expectedStatus)
            {
                if (response.Status == ChangeSetStatus.FAILED)
                {
                    if (response.StatusReason == "No updates are to be performed.")
                    {
                        Console.WriteLine(response.StatusReason);

                        throw new EmptyChangeException();
                    }

                    throw new System.InvalidOperationException($"Unexpected status {response.Status} - {response.StatusReason}");
                }

                Console.WriteLine($"{response.Status}...");

                await Task.Delay(TimeSpan.FromSeconds(15));

                response = await DescribeChangeSetStatus(changeSetId, stackName, credentials);
            }

            Console.WriteLine($"Change set {changeSetId} for stack {stackName} successfully reached {response.Status} status.");
        }

        private static async Task WaitForStackStatus(string stackName, StackStatus expectedStatus, Credentials credentials)
        {
            StackStatus stackStatus = await GetStackStatus(stackName, credentials);

            while (stackStatus != expectedStatus)
            {
                Console.WriteLine($"{stackStatus}...");

                if (!stackStatus.ToString().EndsWith("_IN_PROGRESS"))
                {
                    throw new System.InvalidOperationException($"Unexpected status {stackStatus}");
                }

                await Task.Delay(TimeSpan.FromSeconds(15));

                stackStatus = await GetStackStatus(stackName, credentials);
            }

            Console.WriteLine($"Stack {stackName} successfully reached {stackStatus} status.");
        }

        private class EmptyChangeException : Exception { }
    }
}