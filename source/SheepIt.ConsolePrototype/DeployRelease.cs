using System;
using CommandLine;
using LibGit2Sharp;

namespace SheepIt.ConsolePrototype
{
    [Verb("deploy-release")]
    public class DeployReleaseOptions
    {
        [Option("release-id", Required = true)]
        public int ReleaseId { get; set; }

        [Option("environment", Required = true)]
        public string Environment { get; set; }
    }

    public static class DeployRelease
    {
        public static void Run(DeployReleaseOptions options)
        {
            // get variables from yaml
            // run some hardcoded script with given environment variables

            Console.WriteLine($"Deploying release {options.ReleaseId} to {options.Environment} environment");

            var release = GetReleaseById(options.ReleaseId);

            CheckoutCommit(release.CommitSha);

            Console.WriteLine($"Checked out commit {release.CommitSha}");

            var deploymentId = InsertDeployment(new Deployment
            {
                ReleaseId = release.Id,
                DeployedAt = DateTime.UtcNow,
                EnvironmentId = options.Environment
            });

            Console.WriteLine($"Created deployment {deploymentId}");
        }

        private static Release GetReleaseById(int releaseId)
        {
            using (var database = Database.Open())
            {
                var releaseCollection = database.GetCollection<Release>();

                return releaseCollection.FindById(releaseId);
            }
        }

        private static void CheckoutCommit(string commitSha)
        {
            using (var repository = CurrentRepository.Open())
            {
                Commands.Checkout(repository, commitSha);
            }
        }

        private static int InsertDeployment(Deployment deployment)
        {
            using (var database = Database.Open())
            {
                var deploymentCollection = database.GetCollection<Deployment>();

                var id = deploymentCollection.Insert(deployment);

                return id.AsInt32;
            }
        }
    }
}