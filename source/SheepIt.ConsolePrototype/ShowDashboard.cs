using System;
using System.Globalization;
using System.Linq;
using CommandLine;

namespace SheepIt.ConsolePrototype
{
    [Verb("dashboard")]
    public class ShowDashboardOptions
    {
        [Option('p', "project-id", Required = true)]
        public string ProjectId { get; set; }
    }

    public static class ShowDashboard
    {
        public static void Run(ShowDashboardOptions options)
        {
            var environments = Deployments.GetAll(options)
                .GroupBy(deployment => deployment.EnvironmentId)
                .Select(grouping => new
                {
                    EnvironmentId = grouping.Key,
                    CurrentDeployment = grouping
                        .OrderByDescending(deployment => deployment.DeployedAt)
                        .First()
                })
                .OrderBy(environment => environment.EnvironmentId)
                .ToArray();

            Console.WriteLine(" {0,-20}| {1,-20}| {2,-20}| {3,-20}",
                "environment",
                "deployment date",
                "deployment id",
                "release id"
            );

            Console.WriteLine(" " + new string('-', 20 + 2 + 20 + 2 + 20 + 2 + 20));

            foreach (var environment in environments)
            {
                Console.WriteLine(" {0,-20}| {1,-20}| {2,-20}| {3,-20}",
                    environment.EnvironmentId,
                    environment.CurrentDeployment.DeployedAt.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture),
                    environment.CurrentDeployment.Id,
                    environment.CurrentDeployment.ReleaseId
                );
            }
        }
    }
}