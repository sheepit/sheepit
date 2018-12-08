using System;
using CommandLine;

namespace SheepIt.ConsolePrototype
{
    [Verb("create-project")]
    public class CreateProjectOptions
    {
        [Option('i', "id", Required = true)]
        public string Id { get; set; }

        [Option('r', "repository-url", Required = true)]
        public string RepositoryUrl { get; set; }
    }

    public static class CreateProject
    {
        public static void Run(CreateProjectOptions options)
        {
            using (var database = Database.Open())
            {
                var projectCollection = database.GetCollection<Project>();

                projectCollection.Insert(new Project
                {
                    Id = options.Id,
                    RepositoryUrl = options.RepositoryUrl
                });
            }

            Console.WriteLine($"Created project {options.Id}");
        }
    }
}