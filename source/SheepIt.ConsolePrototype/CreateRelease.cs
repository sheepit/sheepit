using System;
using System.Linq;
using CommandLine;

namespace SheepIt.ConsolePrototype
{
    [Verb("create-release")]
    public class CreateReleaseOptions
    {
    }

    public static class CreateRelease
    {
        public static void Run(CreateReleaseOptions options)
        {
            var currentCommitSha = GetCurrentCommitSha();

            Console.WriteLine($"Current commit SHA is {currentCommitSha}");

            var releaseId = InsertRelease(new Release
            {
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

        private static string GetCurrentCommitSha()
        {
            using (var repository = CurrentRepository.Open())
            {
                return repository.Head.Tip.Sha;
            }
        }
    }
}