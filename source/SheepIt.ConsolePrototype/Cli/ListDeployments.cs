using System.Globalization;
using CommandLine;
using SheepIt.ConsolePrototype.UseCases;
using SheepIt.Utils.Console;
using SheepIt.Utils.Extensions;

namespace SheepIt.ConsolePrototype.Cli
{
    [Verb("list-deployments")]
    public class ListDeploymentsOptions
    {
        [Option('p', "project-id", Required = true)]
        public string ProjectId { get; set; }
    }

    public static class ListDeployments
    {
        public static void Run(ListDeploymentsOptions options)
        {
            var response = ListDeploymentsHandler.Handle(new ListDeploymentsRequest
            {
                ProjectId = options.ProjectId
            });

            ConsoleTable.Containing(response.Deployments)
                .WithColumn("Deployed at", deployment => deployment.DeployedAt.ConsoleFriendlyFormat())
                .WithColumn("Environment ID", deployment => deployment.EnvironmentId)
                .WithColumn("Deployment ID", deployment => deployment.Id.ToString(CultureInfo.InvariantCulture))
                .WithColumn("Release ID", deployment => deployment.ReleaseId.ToString(CultureInfo.InvariantCulture))
                .Show();
        }
    }
}