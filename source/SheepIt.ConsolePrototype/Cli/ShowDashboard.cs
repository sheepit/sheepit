using System;
using System.Globalization;
using CommandLine;
using SheepIt.ConsolePrototype.UseCases;
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
            var response = ShowDashboardHandler.Handle(new ShowDashboardRequest
            {
                ProjectId = options.ProjectId
            });

            ConsoleTable.Containing(response.Environments)
                .WithColumn("Environment ID", environment => environment.EnvironmentId)
                .WithColumn("Deployment date", environment => environment.LastDeployedAt.ConsoleFriendlyFormat())
                .WithColumn("Deployment ID", environment => environment.CurrentDeploymentId.ToString(CultureInfo.InvariantCulture))
                .WithColumn("Release ID", environment => environment.CurrentReleaseId.ToString(CultureInfo.InvariantCulture))
                .Show();
        }
    }
}