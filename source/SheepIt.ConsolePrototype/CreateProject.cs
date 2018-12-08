using System;
using CommandLine;

namespace SheepIt.ConsolePrototype
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
            Projects.Add(new Project
            {
                Id = options.ProjectId,
                RepositoryUrl = options.RepositoryUrl
            });

            Console.WriteLine($"Created project {options.ProjectId}");
        }
    }
}