using System;
using System.Globalization;
using CommandLine;
using LibGit2Sharp;

namespace SheepIt.ConsolePrototype
{
    [Verb("create-release")]
    public class CreateReleaseOptions
    {
        [Option('p', "project-id", Required = true)]
        public string ProjectId { get; set; }
    }

    public static class CreateRelease
    {
        public static void Run(CreateReleaseOptions options)
        {
            var project = Projects.Get(options.ProjectId);
            
            var currentCommitSha = GetCurrentCommitSha(project);

            Console.WriteLine($"Current commit SHA is {currentCommitSha}");

            var releaseId = InsertRelease(new Release
            {
                ProjectId = options.ProjectId,
                CommitSha = currentCommitSha,
                CreatedAt = DateTime.UtcNow
            });

            Console.WriteLine($"Created release {releaseId}");
        }

        private static int InsertRelease(Release release)
        {
            using (var liteDatabase = Database.Open())
            {
                var releases = liteDatabase.GetCollection<Release>();

                var id = releases.Insert(release);

                return id.AsInt32;
            }
        }

        private static string GetCurrentCommitSha(Project project)
        {
            var formattedUtcNow = DateTime.UtcNow.ToString("yyyy-MM-dd_hh-mm-ss", CultureInfo.InvariantCulture);
            var workdirPath = $"./creating-release_{project.Id}_{formattedUtcNow}";

            // todo: we could use git ls-remote to get same information without cloning the repo, when libgit2sharp supports it
            // right not we could do following:
            // https://github.com/libgit2/libgit2sharp/issues/1377#issuecomment-253177481
            // i. e. create a new repo and get info we want

            Repository.Clone(project.RepositoryUrl, workdirPath, new CloneOptions
            {
                BranchName = "master" // todo: should this be configurable?
            });

            using (var repository = new Repository(workdirPath))
            {
                return repository.Head.Tip.Sha;
            }
        }
    }
}