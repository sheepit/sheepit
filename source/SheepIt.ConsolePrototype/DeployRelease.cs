using System;
using System.Linq;
using CommandLine;
using SheepIt.ConsolePrototype.CommandRunners;

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

            var project = Projects.Get(
                projectId: options.ProjectId
            );

            var release = Releases.Get(
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

                var variablesForCurrentEnvironment = variables.GetForEnvironment(options.Environment);

                foreach (var variable in variablesForCurrentEnvironment)
                {
                    Console.WriteLine($"    {variable.Name}: {variable.Value}");
                }

                Console.WriteLine();

                // run process

                var processDescription = repository.OpenProcessDescriptionFile();

                new ProcessRunner().Run(
                    processFile: processDescription,
                    variables: variablesForCurrentEnvironment
                );

                // save deployment

                // todo: we should persist deployments at the beginning and later include information whether it succeeded or not

                var deploymentId = Deployments.Add(new Deployment
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