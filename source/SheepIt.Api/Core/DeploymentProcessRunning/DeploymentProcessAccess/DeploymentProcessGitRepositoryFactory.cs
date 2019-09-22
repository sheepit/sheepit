﻿using System;
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
                    ZipFile = zipFile,
                    CreatedFromCommitSha = repository.GetCurrentCommitSha()
                };
            }
        }

        public class GetCurrentCommitZipResult
        {
            public byte[] ZipFile { get; set; }
            public string CreatedFromCommitSha { get; set; }
        }

        public string GetCurrentCommitSha(Project project)
        {
            using (var repository = Clone(project))
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