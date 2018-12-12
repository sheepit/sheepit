using System.Globalization;
using CommandLine;
using SheepIt.ConsolePrototype.UseCases;
using SheepIt.Utils.Console;

namespace SheepIt.ConsolePrototype.Cli
{
    [Verb("list-projects")]
    public class ListProjectsOptions
    {
    }

    public static class ListProjects
    {
        public static void Run(ListProjectsOptions options)
        {
            var response = ListProjectsHandler.Handle(new ListProjectsRequest());

            ConsoleTable.Containing(response.Projects)
                .WithColumn("Project ID", deployment => deployment.Id.ToString(CultureInfo.InvariantCulture))
                .WithColumn("Repository URL", deployment => deployment.RepositoryUrl)
                .Show();
        }
    }
}