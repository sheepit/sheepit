using System.Globalization;
using System.Linq;
using CommandLine;

namespace SheepIt.ConsolePrototype
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
            using (var database = Database.Open())
            {
                var deploymentCollection = database.GetCollection<Deployment>();

                var deployments = deploymentCollection
                    .Find(deployment => deployment.ProjectIt == options.ProjectId)
                    .OrderBy(deployment => deployment.DeployedAt)
                    .ToArray();

                ConsoleTable.Containing(deployments)
                    .WithColumn("Deployed at", deployment => deployment.DeployedAt.ConsoleFriendlyFormat())
                    .WithColumn("Environment ID", deployment => deployment.EnvironmentId)
                    .WithColumn("Deployment ID", deployment => deployment.Id.ToString(CultureInfo.InvariantCulture))
                    .WithColumn("Release ID", deployment => deployment.ReleaseId.ToString(CultureInfo.InvariantCulture))
                    .Show();
            }
        }
    }
}