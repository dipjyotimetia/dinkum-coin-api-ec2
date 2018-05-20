using Amazon.SecurityToken.Model;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Build.Aws
{
    /// <summary>
    /// Provides methods for interacting with AWS Simple Systems Management.
    /// </summary>
    public static class Ssm
    {
        /// <summary>
        /// Deletes the specified parameters.
        /// </summary>
        /// <param name="names">The names of the parameters to delete.</param>
        /// <param name="credentials">AWS credentials.</param>
        /// <returns>A task that represents the asynchronous deletion operation.</returns>
        /// <exception cref="ParameterOperationException">One or more parameters could not be deleted.</exception>
        public static async Task DeleteParameters(ICollection<string> names, Credentials credentials)
        {
            Console.WriteLine("Deleting SSM parameters...");

            if (!names.Any()) { return; }

            DeleteParametersResponse result = await GetClient(credentials).DeleteParametersAsync(new DeleteParametersRequest { Names = names.ToList() });

            foreach (string parameter in result.DeletedParameters)
            {
                Console.WriteLine($"{parameter}");
            }

            if (result.InvalidParameters.Any())
            {
                throw new ParameterOperationException("Unable to delete invalid SSM parameters: " + string.Join(", ", result.InvalidParameters));
            }
        }

        /// <summary>
        /// Executes the specified automation document.
        /// </summary>
        /// <param name="documentName">The name of the document to execute.</param>
        /// <param name="parameters">The parameters to pass to the automation document.</param>
        /// <param name="credentials">AWS credentials.</param>
        /// <returns>The automation execution ID of the successfully executed document.</returns>
        /// <exception cref="AutomationExecutionException">The execution of the document failed.</exception>
        public static async Task<string> ExecuteAutomation(
            string documentName, IDictionary<string, ICollection<string>> parameters, Credentials credentials)
        {
            Console.WriteLine($"Starting automation document {documentName}...");

            StartAutomationExecutionResponse response = await GetClient(credentials).StartAutomationExecutionAsync(
                new StartAutomationExecutionRequest
                {
                    DocumentName = documentName,
                    Parameters = parameters.ToDictionary(pair => pair.Key, pair => pair.Value.ToList())
                });

            await WaitForAutomation(response.AutomationExecutionId, credentials);

            return response.AutomationExecutionId;
        }

        /// <summary>
        /// Gets the outputs of an execution of an automation document.
        /// </summary>
        /// <param name="automationExecutionId">The ID of the automation execution.</param>
        /// <param name="credentials">AWS credentials.</param>
        /// <returns>A dictionary of output parameters.</returns>
        public static async Task<Dictionary<string, List<string>>> GetAutomationExecutionOutputs(
            string automationExecutionId, Credentials credentials)
        {
            return (await GetClient(credentials)
                .GetAutomationExecutionAsync(new GetAutomationExecutionRequest { AutomationExecutionId = automationExecutionId }))
                .AutomationExecution.Outputs;
        }

        /// <summary>
        /// Gets the value of the specified paramter.
        /// </summary>
        /// <param name="name">The name of the parameter to retrieve.</param>
        /// <param name="credentials">AWS credentials.</param>
        /// <param name="ignoreMissing">true to ignore missing parameters; otherwise, false for an exception to be thrown when parameters are missing</param>
        /// <returns>The value of the specified parameter.</returns>
        /// <exception cref="ParameterNotFoundException">The specified parameter was not found.</exception>
        public static async Task<string> GetParameter(string name, Credentials credentials, bool ignoreMissing = false)
        {
            string parameterValue;

            try
            {
                parameterValue = (await GetClient(credentials).GetParameterAsync(
                    new GetParameterRequest
                    {
                        Name = name,
                        WithDecryption = true
                    })).Parameter.Value;
            }
            catch (ParameterNotFoundException)
            {
                if (ignoreMissing)
                {
                    parameterValue = null;
                }
                else
                {
                    throw;
                }
            }

            return parameterValue;
        }

        /// <summary>
        /// Gets the names of parameters within the specified hierarchy.
        /// </summary>
        /// <param name="path">The path of the parameters to retrieve.</param>
        /// <param name="credentials">AWS credentials.</param>
        /// <returns>The names of the parameters that match the given path.</returns>
        public static async Task<string[]> GetParameterNamesByPath(string path, Credentials credentials)
        {
            return (await GetClient(credentials).GetParametersByPathAsync(new GetParametersByPathRequest { Path = path, Recursive = true }))
                .Parameters.Select(param => param.Name).ToArray();
        }

        /// <summary>
        /// Sets a parameter to the given value.
        /// </summary>
        /// <param name="name">The name of the parameter to set.</param>
        /// <param name="value">The value to set the parameter to.</param>
        /// <param name="credentials">AWS credentials.</param>
        /// <returns>A task that represents the asynchronous parameter operation.</returns>
        public static async Task SetParameter(string name, string value, Credentials credentials)
        {
            await SetParameter(name, value, null, credentials, false);
        }

        /// <summary>
        /// Sets a secure parameter to the given value.
        /// </summary>
        /// <param name="name">The name of the secure parameter to set.</param>
        /// <param name="value">The value to set the secure parameter to.</param>
        /// <param name="keyId">The KMS Key ID that is to be used to encrypt a parameter.
        /// If keyId is null, the system uses the AWS account's default key.</param>
        /// <param name="credentials">AWS credentials.</param>
        /// <returns>A task that represents the asynchronous parameter operation.</returns>
        public static async Task SetSecureParameter(string name, string value, string keyId, Credentials credentials)
        {
            await SetParameter(name, value, keyId, credentials, true);
        }

        private static async Task<AutomationExecutionStatus> GetAutomationStatus(string automationExecutionId, Credentials credentials)
        {
            return (await GetClient(credentials).GetAutomationExecutionAsync(
                new GetAutomationExecutionRequest { AutomationExecutionId = automationExecutionId })).AutomationExecution.AutomationExecutionStatus;
        }

        private static AmazonSimpleSystemsManagementClient GetClient(Credentials credentials)
        {
            return credentials == null ? new AmazonSimpleSystemsManagementClient() : new AmazonSimpleSystemsManagementClient(credentials);
        }

        private static async Task SetParameter(string name, string value, string keyId, Credentials credentials, bool secure)
        {
            await GetClient(credentials).PutParameterAsync(
                new PutParameterRequest
                {
                    Name = name,
                    Value = value,
                    Type = secure ? ParameterType.SecureString : ParameterType.String,
                    KeyId = keyId,
                    Overwrite = true
                });
        }

        private static async Task WaitForAutomation(string automationExecutionId, Credentials credentials)
        {
            AutomationExecutionStatus status = await GetAutomationStatus(automationExecutionId, credentials);

            while (status != AutomationExecutionStatus.Success)
            {
                Console.WriteLine($"{status}...");

                if (status != AutomationExecutionStatus.InProgress && status != AutomationExecutionStatus.Pending)
                {
                    throw new AutomationExecutionException($"Automation execution entered {status} status", automationExecutionId);
                }

                await Task.Delay(TimeSpan.FromSeconds(15));

                status = await GetAutomationStatus(automationExecutionId, credentials);
            }

            Console.WriteLine("Automation execution was successful.");
        }
    }
}