using System.Globalization;
using System.Linq;
using CommandLine;

namespace SheepIt.ConsolePrototype
{
    [Verb("list-projects")]
    public class ListProjectsOptions
    {
    }

    public static class ListProjects
    {
        public static void Run(ListProjectsOptions options)
        {
            using (var database = Database.Open())
            {
                var deploymentCollection = database.GetCollection<Project>();

                var deployments = deploymentCollection
                    .FindAll()
                    .OrderBy(deployment => deployment.Id)
                    .ToArray();

                ConsoleTable.Containing(deployments)
                    .WithColumn("Project ID", deployment => deployment.Id.ToString(CultureInfo.InvariantCulture))
                    .WithColumn("Repository URL", deployment => deployment.RepositoryUrl)
                    .Show();
            }
        }
    }
}