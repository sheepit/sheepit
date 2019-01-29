using System;
using LibGit2Sharp;
using SheepIt.Api.Infrastructure;
using SheepIt.Domain;
using SheepIt.Utils.Extensions;

namespace SheepIt.Api.Infrastructure
{
    public class ProcessRepositoryFactory
    {
        private readonly ProcessSettings _processSettings;

        public ProcessRepositoryFactory(ProcessSettings processSettings)
        {
            _processSettings = processSettings;
        }

        public string GetCurrentCommitSha(Project project)
        {
            var repositoryWorkingDir = _processSettings.WorkingDir
                .AddSegment(project.Id)
                .AddSegment("creating-releases")
                .AddSegment($"{DateTime.UtcNow.FileFriendlyFormat()}");

            using (var repository = Clone(project.RepositoryUrl, repositoryWorkingDir.ToString()))
            {
                // todo: we shouldn't clone whole repo to just get a commit
                // git ls-remote can get same information without cloning the entire repo
                // libgit2sharp doesn't support it yet (although libgit2 does)
                // workaround would be to create a new repo and get info we want:
                // https://github.com/libgit2/libgit2sharp/issues/1377#issuecomment-253177481

                // todo: setting branch/tag/commit should be configurable when creating a release

                return repository.GetCurrentCommitSha();
            }
        }

        public ProcessRepository Clone(string repositoryUrl, string toDirectory)
        {
            Repository.Clone(repositoryUrl, toDirectory, new CloneOptions
            {
                BranchName = "master" // todo: should this be configurable?
            });

            var repository = new Repository(toDirectory);

            return new ProcessRepository(repository);
        }
    }
}