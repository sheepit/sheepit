using System;
using LibGit2Sharp;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.Infrastructure.Time;

namespace SheepIt.Api.Core.DeploymentProcessRunning.DeploymentProcessAccess
{
    public class DeploymentProcessGitRepositoryFactory
    {
        private readonly DeploymentProcessSettings _deploymentProcessSettings;

        public DeploymentProcessGitRepositoryFactory(DeploymentProcessSettings deploymentProcessSettings)
        {
            _deploymentProcessSettings = deploymentProcessSettings;
        }

        public GetCurrentCommitZipResult GetCurrentCommitZip(Project project)
        {
            using (var repository = Clone(project))
            {
                var zipFile = repository.Zip();

                return new GetCurrentCommitZipResult
                {
                    ZipFile = zipFile
                };
            }
        }

        public class GetCurrentCommitZipResult
        {
            public byte[] ZipFile { get; set; }
        }

        public DeploymentProcessGitRepository Clone(Project project)
        {
            var repositoryWorkingDir = _deploymentProcessSettings.WorkingDir
                .AddSegment(project.Id)
                .AddSegment("creating-releases")
                .AddSegment($"{DateTime.UtcNow.FileFriendlyFormat()}")
                .AddSegment("repository");

            return Clone(project.RepositoryUrl, repositoryWorkingDir.ToString());
        }
        
        public DeploymentProcessGitRepository Clone(string repositoryUrl, string toDirectory)
        {
            Repository.Clone(repositoryUrl, toDirectory, new CloneOptions
            {
                BranchName = "master" // todo: should this be configurable?
            });
            
            var repository = new Repository(toDirectory);
            
            return new DeploymentProcessGitRepository(repository);
        }
    }
}