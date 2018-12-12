using System.Globalization;
using CommandLine;
using SheepIt.ConsolePrototype.UseCases;
using SheepIt.Utils.Console;
using SheepIt.Utils.Extensions;

namespace SheepIt.ConsolePrototype.Cli
{
    [Verb("list-releases")]
    public class ListReleasesOptions
    {
        [Option('p', "project-id", Required = true)]
        public string ProjectId { get; set; }
    }

    public static class ListReleases
    {
        public static void Run(ListReleasesOptions options)
        {
            var response = ListReleasesHandler.Handle(new ListReleasesRequest
            {
                ProjectId = options.ProjectId
            });

            ConsoleTable.Containing(response.Releases)
                .WithColumn("Release ID", release => release.Id.ToString(CultureInfo.InvariantCulture))
                .WithColumn("Created at", release => release.CreatedAt.ConsoleFriendlyFormat())
                .WithColumn("Commit SHA", release => release.CommitSha)
                .Show();
        }
    }
}