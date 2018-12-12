using System;
using SheepIt.ConsolePrototype.Infrastructure;
using SheepIt.Domain;
using SheepIt.Utils.Extensions;

namespace SheepIt.ConsolePrototype.UseCases
{
    public class CreateReleaseRequest
    {
        public string ProjectId { get; set; }
    }

    public class CreateReleaseResponse
    {
        public int CreatedReleaseId { get; set; }
        public string CreatedFromCommitSha { get; set; }
    }

    public static class CreateReleaseHandler
    {
        public static CreateReleaseResponse Handle(CreateReleaseRequest request)
        {
            var project = Projects.Get(request.ProjectId);

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
                
                var releaseId = Releases.Add(new Release
                {
                    ProjectId = request.ProjectId,
                    CommitSha = currentCommitSha,
                    CreatedAt = DateTime.UtcNow
                });

                return new CreateReleaseResponse
                {
                    CreatedReleaseId = releaseId,
                    CreatedFromCommitSha = currentCommitSha
                };
            }
        }
    }
}