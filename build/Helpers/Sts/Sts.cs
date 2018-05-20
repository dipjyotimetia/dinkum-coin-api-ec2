using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Build.Aws
{
    /// <summary>
    /// Provides methods for accessing AWS Security Token Service (STS). The AWS Security Token Service (STS)
    /// is a web service that enables you to request temporary, limited-privilege credentials for AWS Identity
    /// and Access Management (IAM) users or for users that you authenticate (federated users).
    /// </summary>
    public static class Sts
    {
        private const string IamRoleArn = "arn:aws:iam::{0}:role/{1}";

        /// <summary>
        /// Creates a set of temporary credentials to gain access to AWS resources.
        /// </summary>
        /// <param name="roleArn">The ARN of the role to be assumed.</param>
        /// <param name="sessionName">The name of the assumed role session.</param>
        /// <param name="tokenCode">The value provided by an MFA device.</param>
        /// <param name="serialNumber">The identification number of the MFA device that is associated with the user who is making the AssumeRole call.</param>
        /// <param name="credentials">The set of AWS credentials to assume a role from.</param>
        /// <returns>A temporary set of credentials for accessing AWS resources</returns>
        public static async Task<Credentials> AssumeRole(string roleArn, string sessionName, string tokenCode = null, string serialNumber = null, Credentials credentials = null)
        {
            return (await GetClient(credentials).AssumeRoleAsync(
                new AssumeRoleRequest
                {
                    RoleArn = roleArn,
                    TokenCode = tokenCode,
                    SerialNumber = serialNumber,
                    RoleSessionName = sessionName
                })).Credentials;
        }

        /// <summary>
        /// Creates a set of temporary credentials to gain access to AWS resources for users who have been authenticated via a SAML authentication response.
        /// </summary>
        /// <param name="roleArn">The ARN of the role to be assumed.</param>
        /// <param name="principalArn">The ARN of the SAML provider in IAM that describes the IdP.</param>
        /// <param name="samlAssertion">The base-64 encoded SAML authentication response provided by the IdP.</param>
        /// <param name="credentials">The set of AWS credentials to assume a role from.</param>
        /// <returns>A temporary set of credentials for accessing AWS resources</returns>
        public static async Task<Credentials> AssumeRoleWithSaml(string roleArn, string principalArn, string samlAssertion, Credentials credentials = null)
        {
            return (await GetClient(credentials).AssumeRoleWithSAMLAsync(
                new AssumeRoleWithSAMLRequest
                {
                    RoleArn = roleArn,
                    PrincipalArn = principalArn,
                    SAMLAssertion = samlAssertion
                })).Credentials;
        }

        /// <summary>
        /// Gets the AWS Account number from the identity whose credentials are used to call the API.
        /// </summary>
        /// <param name="credentials">The set of AWS credentials to retrieve the account number from.</param>
        /// <returns>The AWS Account number from caller identity</returns>
        public static async Task<string> GetCallerAccount(Credentials credentials = null)
        {
            return (await GetClient(credentials).GetCallerIdentityAsync(new GetCallerIdentityRequest())).Account;
        }

        /// <summary>
        /// Gets the ARN from the identity whose credentials are used to call the API.
        /// </summary>
        /// <param name="credentials">The set of AWS credentials to retrieve the ARN from.</param>
        /// <returns>The AWS ARN associated with the calling identity</returns>
        public static async Task<string> GetCallerArn(Credentials credentials = null)
        {
            return (await GetClient(credentials).GetCallerIdentityAsync(new GetCallerIdentityRequest())).Arn;
        }

        /// <summary>
        /// Gets the IAM ARN from the identity whose credentials are used to call the API.
        /// </summary>
        /// <param name="credentials">The set of AWS credentials to retrieve the IAM ARN from.</param>
        /// <returns>The AWS IAM ARN associated with the calling identity</returns>
        public static string GetCallerIamArn(Credentials credentials = null)
        {
            string assumedRoleArn = GetCallerArn(credentials).Result;

            Regex assumedRolePattern = new Regex(@"arn:aws:sts::(?<accountNumber>\d+):assumed-role/(?<roleName>.*)/.*");
            Regex iamRolePattern = new Regex(@"arn:aws:iam::.*");

            var match = assumedRolePattern.Match(assumedRoleArn);

            if (match.Success)
            {
                string accountNumber = match.Groups["accountNumber"].Value;
                string roleName = match.Groups["roleName"].Value;

                return string.Format(IamRoleArn, accountNumber, roleName);
            }

            match = iamRolePattern.Match(assumedRoleArn);

            if (match.Success)
            {
                return assumedRoleArn;
            }

            throw new StsArnException("Unable to retrieve IAM ARN");
        }

        private static AmazonSecurityTokenServiceClient GetClient(Credentials credentials)
        {
            return credentials == null ? new AmazonSecurityTokenServiceClient() : new AmazonSecurityTokenServiceClient(credentials);
        }
    }
}