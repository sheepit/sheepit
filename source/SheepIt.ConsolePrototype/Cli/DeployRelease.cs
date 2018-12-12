using System;
using System.Linq;
using CommandLine;
using SheepIt.ConsolePrototype.UseCases;

namespace SheepIt.ConsolePrototype.Cli
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

            var response = DeployReleaseHandler.Handle(new DeployReleaseRequest
            {
                ProjectId = options.ProjectId,
                ReleaseId = options.ReleaseId,
                Environment = options.Environment
            });

            Console.WriteLine($"Checked out commit {response.FromCommitSha}");
            Console.WriteLine();

            Console.WriteLine("Deploying using following variables:");

            foreach (var variable in response.UsedVariables)
            {
                Console.WriteLine($"    {variable.Key}: {variable.Value}");
            }

            Console.WriteLine();

            Console.WriteLine("Deployment output:");

            foreach (var outputLine in response.ProcessResult.Steps.SelectMany(result => result.Output))
            {
                Console.WriteLine($"    {outputLine}");
            }

            Console.WriteLine();

            Console.WriteLine($"Created deployment {response.CreatedDeploymentId}");
            Console.WriteLine();
        }
    }
}