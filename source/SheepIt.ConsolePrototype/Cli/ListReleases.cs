using System.Globalization;
using System.Linq;
using CommandLine;
using SheepIt.Domain;
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
            using (var database = Database.Open())
            {
                var releaseCollection = database.GetCollection<Release>();

                var releases = releaseCollection
                    .Find(release => release.ProjectId == options.ProjectId)
                    .OrderBy(release => release.CreatedAt)
                    .ToArray();

                ConsoleTable.Containing(releases)
                    .WithColumn("Release ID", release => release.Id.ToString(CultureInfo.InvariantCulture))
                    .WithColumn("Created at", release => release.CreatedAt.ConsoleFriendlyFormat())
                    .WithColumn("Commit SHA", release => release.CommitSha)
                    .Show();
            }
        }
    }
}