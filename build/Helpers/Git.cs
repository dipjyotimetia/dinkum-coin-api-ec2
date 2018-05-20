using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Build
{
    public static class Git
    {
        public static bool BranchExists(string repoPath, string branchName)
        {
            using (var repo = new Repository(repoPath))
            {
                return repo.Branches[branchName] != null;
            }
        }

        public static void CheckoutPaths(string repoPath, string branch, params string[] pathSpecs)
        {
            using (var repo = new Repository(repoPath))
            {
                repo.CheckoutPaths(
                    branch, pathSpecs,
                    new CheckoutOptions
                    {
                        CheckoutNotifyFlags = CheckoutNotifyFlags.Updated,
                        OnCheckoutNotify = (path, flags) =>
                        {
                            Console.WriteLine($"Checking out {path}...");

                            return true;
                        }
                    });
            }
        }

        public static string GetBranchName(string repoPath)
        {
            string branchName = Environment.GetEnvironmentVariable("BRANCH_NAME");

            if (branchName != null)
            {
                return branchName;
            }

            branchName = Environment.GetEnvironmentVariable("GIT_BRANCH");

            if (branchName != null)
            {
                return branchName.Substring(7);
            }

            using (var repo = new Repository(repoPath))
            {
                return repo.Head.FriendlyName;
            }
        }

        public static int GetCommitCount(string repoPath)
        {
            using (var repo = new Repository(repoPath))
            {
                return repo.Head.Commits.Count();
            }
        }

        public static void InitialiseRepository(string repoPath, string gitUrl, string userName, string password)
        {
            Console.WriteLine($"\nInitialising Git repository: {repoPath}");

            Repository.Init(repoPath);

            using (var repo = new Repository(repoPath))
            {
                Remote remote = repo.Network.Remotes.Add("origin", gitUrl);
                IEnumerable<string> refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);

                Commands.Fetch(
                    repo, remote.Name, refSpecs,
                    new FetchOptions
                    {
                        CredentialsProvider = (url, usernameFromUrl, types) => new UsernamePasswordCredentials { Password = password, Username = userName }
                    }, "");
            }
        }
    }
}