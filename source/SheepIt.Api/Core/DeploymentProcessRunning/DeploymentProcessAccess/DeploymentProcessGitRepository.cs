using System;
using System.IO;
using System.IO.Compression;
using LibGit2Sharp;
using SheepIt.Api.Infrastructure;

namespace SheepIt.Api.Core.DeploymentProcessRunning.DeploymentProcessAccess
{
    // todo: move to some other namespace
    public class DeploymentProcessGitRepository : IDisposable
    {
        private readonly Repository _repository;

        public DeploymentProcessGitRepository(Repository repository)
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

        public byte[] Zip()
        {
            var zipPath = RepositoryPath
                .AddSegment("..")
                .AddSegment("repository.zip")
                .ToString();
            
            ZipFile.CreateFromDirectory(
                sourceDirectoryName: RepositoryPath.ToString(),
                destinationArchiveFileName: zipPath
            );

            return File.ReadAllBytes(zipPath);
        }

        public DeploymentProcessFile OpenProcessDescriptionFile()
        {
            var processDescriptionFilePath = RepositoryPath.AddSegment("process.yaml").ToString();

            return DeploymentProcessFile.Open(processDescriptionFilePath);
        }

        public LocalPath RepositoryPath => new LocalPath(_repository.Info.WorkingDirectory);

        public void Dispose()
        {
            _repository?.Dispose();
        }
    }
}