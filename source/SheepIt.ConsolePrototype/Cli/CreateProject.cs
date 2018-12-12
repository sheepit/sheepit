using System;
using CommandLine;
using SheepIt.ConsolePrototype.UseCases;

namespace SheepIt.ConsolePrototype.Cli
{
    [Verb("create-project")]
    public class CreateProjectOptions
    {
        [Option('p', "project-id", Required = true)]
        public string ProjectId { get; set; }

        [Option('r', "repository-url", Required = true)]
        public string RepositoryUrl { get; set; }
    }

    public static class CreateProject
    {
        public static void Run(CreateProjectOptions options)
        {
            CreateProjectHandler.Handle(new CreateProjectRequest
            {
                ProjectId = options.ProjectId,
                RepositoryUrl = options.RepositoryUrl
            });

            Console.WriteLine($"Created project {options.ProjectId}");
        }
    }
}