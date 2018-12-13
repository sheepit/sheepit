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
        public string Environment { get; set; }
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

            var deploymentWorkingDir = Settings.WorkingDir
                .AddSegment(project.Id)
                .AddSegment("deploying-releases")
                .AddSegment($"{DateTime.UtcNow.FileFriendlyFormat()}_{request.Environment}_release-{release.Id}")
                .ToString();

            using (var repository = ProcessRepository.Clone(project.RepositoryUrl, deploymentWorkingDir))
            {
                // checkout

                repository.Checkout(release.CommitSha);

                // read variables

                var variables = release.GetVariablesForEnvironment(request.Environment);

                Console.WriteLine();

                // run process

                var processResult = new ProcessRunner().Run(
                    processFile: repository.OpenProcessDescriptionFile(),
                    variablesForEnvironment: variables,
                    workingDir: deploymentWorkingDir
                );

                // save deployment

                // todo: we should persist deployments at the beginning and later include information whether it succeeded or not
                var deploymentId = Deployments.Add(new Deployment
                {
                    ReleaseId = release.Id,
                    ProjectIt = request.ProjectId,
                    DeployedAt = DateTime.UtcNow,
                    EnvironmentId = request.Environment
                });

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