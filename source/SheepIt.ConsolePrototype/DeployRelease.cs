using System;
using CommandLine;

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

            var release = Releases.GetRelease(
                projectId: options.ProjectId,
                releaseId: options.ReleaseId
            );

            var deploymentWorkingDir = Settings.WorkingDir
                .AddSegment(project.Id)
                .AddSegment("deploying-releases")
                .AddSegment($"{DateTime.UtcNow.FileFriendlyFormat()}_{options.Environment}_release-{release.Id}")
                .ToString();

            using (var repository = ProcessRepository.Clone(project.RepositoryUrl, deploymentWorkingDir))
            {
                // checkout

                repository.Checkout(release.CommitSha);

                Console.WriteLine($"Checked out commit {release.CommitSha}");
                Console.WriteLine();

                // read variables

                var variables = repository.OpenVariableFile();

                Console.WriteLine("Deploying using following variables:");

                foreach (var variable in variables.GetForEnvironment(options.Environment))
                {
                    Console.WriteLine($"    {variable.Name}: {variable.Value}");
                }

                Console.WriteLine();

                // run process

                var processDescription = repository.OpenProcessDescriptionFile();

                Console.WriteLine($"Running deployment script: {processDescription.Script}");
                Console.WriteLine();

                // save deployment

                // todo: we should persist deployments at the beginning and later include information whether it succeeded or not

                var deploymentId = Deployments.InsertDeployment(new Deployment
                {
                    ReleaseId = release.Id,
                    ProjectIt = options.ProjectId,
                    DeployedAt = DateTime.UtcNow,
                    EnvironmentId = options.Environment
                });

                Console.WriteLine($"Created deployment {deploymentId}");
                Console.WriteLine();
            }
        }
    }
}