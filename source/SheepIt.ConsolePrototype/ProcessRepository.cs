using System;
using LibGit2Sharp;

namespace SheepIt.ConsolePrototype
{
    public class ProcessRepository : IDisposable
    {
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

        public VariablesFile OpenVariableFile()
        {
            var variablesFilePath = RepositoryPath.AddSegment("variables.yaml").ToString();

            return VariablesFile.Open(variablesFilePath);
        }

        public ProcessDescription OpenProcessDescriptionFile()
        {
            var processDescriptionFilePath = RepositoryPath.AddSegment("process.yaml").ToString();

            return ProcessDescriptionFile.Open(processDescriptionFilePath);
        }

        private LocalPath RepositoryPath => new LocalPath(_repository.Info.WorkingDirectory);

        public void Dispose()
        {
            _repository?.Dispose();
        }
    }
}