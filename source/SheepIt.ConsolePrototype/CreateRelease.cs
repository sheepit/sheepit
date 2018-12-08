using System;
using CommandLine;

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

            var repositoryWorkingDir = Settings.WorkingDir
                .AddSegment(project.Id)
                .AddSegment("creating-releases")
                .AddSegment($"{DateTime.UtcNow.FileFriendlyFormat()}");

            using (var repository = ProcessRepository.Clone(project.RepositoryUrl, repositoryWorkingDir.ToString()))
            {
                // todo: we shouldn't clone whole repo to just get a commit
                // git ls-remote can get same information without cloning the entire repo
                // libgit2sharp doesn't support it yet (although libgit2 does)
                // workaround would be to create a new repo and get info we want:
                // https://github.com/libgit2/libgit2sharp/issues/1377#issuecomment-253177481

                // todo: setting branch/tag/commit should be configurable when creating a release
                
                var currentCommitSha  = repository.GetCurrentCommitSha();

                Console.WriteLine($"Current commit SHA is {currentCommitSha}");

                var releaseId = InsertRelease(new Release
                {
                    ProjectId = options.ProjectId,
                    CommitSha = currentCommitSha,
                    CreatedAt = DateTime.UtcNow
                });

                Console.WriteLine($"Created release {releaseId}");
            }
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
    }
}