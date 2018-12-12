using System;
using CommandLine;
using SheepIt.ConsolePrototype.UseCases;

namespace SheepIt.ConsolePrototype.Cli
{
    [Verb("create-release")]
    public class CreateReleaseOptions
    {
        [Option('p', "project-id", Required = true)]
        public string ProjectId { get; set; }
    }

    public static class CreateRelease
    {
        public static void Run(CreateReleaseOptions options)
        {
            var response = CreateReleaseHandler.Handle(new CreateReleaseRequest
            {
                ProjectId = options.ProjectId
            });

            Console.WriteLine($"Created release {response.CreatedFromCommitSha} from commit {response.CreatedFromCommitSha}");
        }
    }
}