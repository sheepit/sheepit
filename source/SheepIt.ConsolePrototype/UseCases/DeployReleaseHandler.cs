using System;
using System.Collections.Generic;
using System.Linq;
using SheepIt.ConsolePrototype.CommandRunners;
using SheepIt.ConsolePrototype.Infrastructure;
using SheepIt.Domain;
using SheepIt.Utils.Extensions;

namespace SheepIt.ConsolePrototype.UseCases
{
    public class DeployReleaseRequest
    {
        public string ProjectId { get; set; }
        public int ReleaseId { get; set; }
        public string EnvironmentId { get; set; }
    }

    public class DeployReleaseResponse
    {
        public int CreatedDeploymentId { get; set; }
        public string FromCommitSha { get; set; }
        public Dictionary<string, string> UsedVariables { get; set; }
        public ProcessResult ProcessResult { get; set; }
    }

    public static class DeployReleaseHandler
    {
        public static DeployReleaseResponse Handle(DeployReleaseRequest request)
        {
            var project = Projects.Get(
                projectId: request.ProjectId
            );

            var release = ReleasesStorage.Get(
                projectId: request.ProjectId,
                releaseId: request.ReleaseId
            );
            
            var deploymentId = Deployments.Add(new Deployment
            {
                ReleaseId = release.Id,
                ProjectId = request.ProjectId,
                DeployedAt = DateTime.UtcNow,
                EnvironmentId = request.EnvironmentId,
                Status = DeploymentStatus.InProgress
            });

            try
            {
                var response = Deploy(project, release, deploymentId, request.EnvironmentId);
               
                Deployments.ChangeDeploymentStatus(deploymentId, DeploymentStatus.Succeeded);
                
                return response;
            }
            catch (Exception)
            {
                Deployments.ChangeDeploymentStatus(deploymentId, DeploymentStatus.Failed);
                
                throw;
            }
        }

        private static DeployReleaseResponse Deploy(Project project, Release release, int deploymentId, string environmentId)
        {
            var variables = release.GetVariablesForEnvironment(environmentId);
            
            var deploymentWorkingDir = Settings.WorkingDir
                .AddSegment(project.Id)
                .AddSegment("deploying-releases")
                .AddSegment($"{DateTime.UtcNow.FileFriendlyFormat()}_{environmentId}_release-{release.Id}")
                .ToString();

            using (var repository = ProcessRepository.Clone(project.RepositoryUrl, deploymentWorkingDir))
            {
                repository.Checkout(release.CommitSha);

                var processResult = new ProcessRunner().Run(
                    processFile: repository.OpenProcessDescriptionFile(),
                    variablesForEnvironment: variables,
                    workingDir: deploymentWorkingDir
                );

                return new DeployReleaseResponse
                {
                    CreatedDeploymentId = deploymentId,
                    FromCommitSha = release.CommitSha,
                    UsedVariables = variables.ToDictionary(
                        keySelector: variable => variable.Name,
                        elementSelector: variable => variable.Value
                    ),
                    ProcessResult = processResult
                };
            }
        }
    }
}