using System;
using LibGit2Sharp;
using Microsoft.Extensions.Configuration;
using SheepIt.Api.ScriptFiles;
using SheepIt.Domain;
using SheepIt.Utils.Extensions;

namespace SheepIt.Api.Infrastructure
{
    public class ProcessRepository : IDisposable
    {
        public static string GetCurrentCommitSha(Project project, IConfiguration configuration)
        {
            var workingDirectoryPath = configuration["WorkingDirectory"];
            var workingDirectory = new LocalPath(workingDirectoryPath);
            
            var repositoryWorkingDir = workingDirectory
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

                return repository.GetCurrentCommitSha();
            }
        }

        public static ProcessRepository Clone(string repositoryUrl, string toDirectory)
        {
            Repository.Clone(repositoryUrl, toDirectory, new CloneOptions
            {
                BranchName = "master" // todo: should this be configurable?
            });

            var repository = new Repository(toDirectory);

            return new ProcessRepository(repository);
        }

        private readonly Repository _repository;

        private ProcessRepository(Repository repository)
        {
            _repository = repository;
        }

        public string GetCurrentCommitSha()
        {
            return _repository.Head.Tip.Sha;
        }

        public void Checkout(string commitSha)
        {
            Commands.Checkout(_repository, commitSha);
        }

        public ProcessFile OpenProcessDescriptionFile()
        {
            var processDescriptionFilePath = RepositoryPath.AddSegment("process.yaml").ToString();

            return ProcessFile.Open(processDescriptionFilePath);
        }

        private LocalPath RepositoryPath => new LocalPath(_repository.Info.WorkingDirectory);

        public void Dispose()
        {
            _repository?.Dispose();
        }
    }
}