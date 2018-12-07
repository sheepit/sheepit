using System;
using CommandLine;
using LibGit2Sharp;

namespace SheepIt.ConsolePrototype
{
    [Verb("deploy-release")]
    public class DeployReleaseOptions
    {
        [Option('r', "release-id", Required = true)]
        public int ReleaseId { get; set; }

        [Option('e', "environment", Required = true)]
        public string Environment { get; set; }
    }

    public static class DeployRelease
    {
        public static void Run(DeployReleaseOptions options)
        {
            Console.WriteLine($"Deploying release {options.ReleaseId} to {options.Environment} environment");
            Console.WriteLine();


            var release = GetReleaseById(options.ReleaseId);


            CheckoutCommit(release.CommitSha);

            Console.WriteLine($"Checked out commit {release.CommitSha}");
            Console.WriteLine();


            var variables = VariablesFile.Open();

            Console.WriteLine("Deploying using following variables:");

            foreach (var variable in variables.GetForEnvironment(options.Environment))
            {
                Console.WriteLine($"    {variable.Name}: {variable.Value}");
            }

            Console.WriteLine();


            var processDescription = ProcessDescriptionFile.Open();

            Console.WriteLine($"Running deployment script: {processDescription.Script}");
            Console.WriteLine();

            var deploymentId = InsertDeployment(new Deployment
            {
                ReleaseId = release.Id,
                DeployedAt = DateTime.UtcNow,
                EnvironmentId = options.Environment
            });

            Console.WriteLine($"Created deployment {deploymentId}");
            Console.WriteLine();
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