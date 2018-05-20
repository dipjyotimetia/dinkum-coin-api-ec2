using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.SecurityToken.Model;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Build.Aws
{
    public static class S3
    {
        public static async Task UploadDirectory(string directory, string bucketName, string keyPrefix, bool recursive = false, Credentials credentials = null)
        {
            var request = new TransferUtilityUploadDirectoryRequest
            {
                BucketName = bucketName,
                Directory = directory,
                KeyPrefix = keyPrefix,
                SearchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly
            };

            request.UploadDirectoryFileRequestEvent += (sender, args) =>
                Console.WriteLine($"Uploading \"{args.UploadRequest.FilePath}\" to S3 bucket \"{bucketName}\"...");

            using (var transferUtility = new TransferUtility(GetClient(credentials)))
            {
                await transferUtility.UploadDirectoryAsync(request);
            }
        }

        private static AmazonS3Client GetClient(Credentials credentials)
        {
            return credentials == null ? new AmazonS3Client() : new AmazonS3Client(credentials);
        }
    }
}