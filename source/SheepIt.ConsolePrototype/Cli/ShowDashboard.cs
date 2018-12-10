using System.Globalization;
using System.Linq;
using CommandLine;
using SheepIt.Domain;
using SheepIt.Utils.Console;
using SheepIt.Utils.Extensions;

namespace SheepIt.ConsolePrototype.Cli
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
            var environments = Deployments.GetAll(options.ProjectId)
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

            ConsoleTable.Containing(environments)
                .WithColumn("Environment ID", environment => environment.EnvironmentId)
                .WithColumn("Deployment date", environment => environment.CurrentDeployment.DeployedAt.ConsoleFriendlyFormat())
                .WithColumn("Deployment ID", environment => environment.CurrentDeployment.Id.ToString(CultureInfo.InvariantCulture))
                .WithColumn("Release ID", environment => environment.CurrentDeployment.ReleaseId.ToString(CultureInfo.InvariantCulture))
                .Show();
        }
    }
}