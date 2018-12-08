using System;
using System.Globalization;
using System.IO;
using CommandLine;
using LibGit2Sharp;

namespace SheepIt.ConsolePrototype
{
    [Verb("deploy-release")]
    public class DeployReleaseOptions
    {
        [Option('p', "project-id", Required = true)]
        public string ProjectId { get; set; }

        [Option('r', "release-id", Required = true)]
        public int ReleaseId { get; set; }

        [Option('e', "environment", Required = true)]
        public string Environment { get; set; }
    }

    public static class DeployRelease
    {
        public static void Run(DeployReleaseOptions options)
        {
            Console.WriteLine($"Deploying project {options.ProjectId}, release {options.ReleaseId} to {options.Environment} environment");
            Console.WriteLine();


            var project = Projects.Get(options.ProjectId);

            // todo: we should also verify project id
            var release = Releases.GetReleaseById(options.ReleaseId);


            var formattedUtcNow = DateTime.UtcNow.ToString("yyyy-MM-dd_hh-mm-ss", CultureInfo.InvariantCulture);
            var workdirPath = $"./deploying-release_{project.Id}_{options.Environment}_{formattedUtcNow}";


            // todo: can we do this with one command?
            CloneRepo(project, workdirPath);
            CheckoutCommit(release.CommitSha, workdirPath);

            Directory.SetCurrentDirectory(workdirPath);

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
                ProjectIt = options.ProjectId,
                DeployedAt = DateTime.UtcNow,
                EnvironmentId = options.Environment
            });

            Console.WriteLine($"Created deployment {deploymentId}");
            Console.WriteLine();
        }

        private static void CloneRepo(Project project, string workdirPath)
        {
            Repository.Clone(project.RepositoryUrl, workdirPath, new CloneOptions
            {
                BranchName = "master" // todo: should this be configurable?
            });
        }

        private static void CheckoutCommit(string commitSha, string workdirPath)
        {
            using (var repository = new Repository(workdirPath))
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