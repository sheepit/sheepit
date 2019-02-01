using System;
using LibGit2Sharp;
using SheepIt.Api.ScriptFiles;

namespace SheepIt.Api.Infrastructure
{
    // todo: move to some other namespace
    public class ProcessRepository : IDisposable
    {
        private readonly Repository _repository;

        public ProcessRepository(Repository repository)
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