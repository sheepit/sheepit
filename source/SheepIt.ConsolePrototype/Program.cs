using System;
using System.Linq;
using CommandLine;
using LiteDB;

namespace SheepIt.ConsolePrototype
{
    [Verb("create-release")]
    public class CreateReleaseOptions
    {
    }

    [Verb("deploy-release")]
    public class DeployReleaseOptions
    {
        [Option("release-id", Default = true)]
        public int ReleaseId { get; set; }
    }

    [Verb("show-dashboard")]
    public class ShowDashboardOptions
    {
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default
                .ParseArguments<CreateReleaseOptions, DeployReleaseOptions, ShowDashboardOptions>(args)
                .WithParsed<CreateReleaseOptions>(CreateRelease)
                .WithParsed<DeployReleaseOptions>(DeployRelease)
                .WithParsed<ShowDashboardOptions>(ShowDashboard)
                .WithNotParsed(errors => {});
        }

        private static void CreateRelease(CreateReleaseOptions options)
        {
            using (var liteDatabase = new LiteDatabase(@"C:\sheep-it\poc-database.db"))
            {
                var releases = liteDatabase.GetCollection<Release>();

                var allReleases = releases.FindAll().ToArray();

                foreach (var release in allReleases)
                {
                    Console.WriteLine($"release {release.Id}: createdAt: {release.CreatedAt}, commitHash: {release.CommitHash}");
                }

                //releases.Insert(new Release
                //{
                //    CommitHash = "123",
                //    CreatedAt = DateTime.UtcNow
                //});
            }
        }

        private static void DeployRelease(DeployReleaseOptions options)
        {
            throw new NotImplementedException();
        }

        private static void ShowDashboard(ShowDashboardOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
