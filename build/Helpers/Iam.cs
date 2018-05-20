using Amazon.IdentityManagement;
using Amazon.IdentityManagement.Model;
using Amazon.SecurityToken.Model;
using System.Linq;
using System.Threading.Tasks;

namespace Build.Aws
{
    public static class Iam
    {
        public static async Task<string> GetSerialNumber(Credentials credentials = null)
        {
            return (await GetClient(credentials).ListMFADevicesAsync(new ListMFADevicesRequest())).MFADevices.First().SerialNumber;
        }

        private static AmazonIdentityManagementServiceClient GetClient(Credentials credentials)
        {
            return credentials == null ? new AmazonIdentityManagementServiceClient() : new AmazonIdentityManagementServiceClient(credentials);
        }
    }
}