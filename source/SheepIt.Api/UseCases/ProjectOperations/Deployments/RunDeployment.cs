using System;
using SheepIt.Api.Infrastructure.Time;
using SheepIt.Api.Model.DeploymentProcesses;
using SheepIt.Api.Model.Deployments;
using SheepIt.Api.Model.Packages;
using SheepIt.Api.Model.Projects;
using SheepIt.Api.Runner.DeploymentProcessRunning;
using SheepIt.Api.Runner.DeploymentProcessRunning.DeploymentProcessAccess;

namespace SheepIt.Api.UseCases.ProjectOperations.Deployments
{
    public class RunDeployment
    {
        private readonly DeploymentProcessSettings _deploymentProcessSettings;
        private readonly DeploymentProcessRunner _deploymentProcessRunner;
        private readonly DeploymentProcessDirectoryFactory _deploymentProcessDirectoryFactory;
        private readonly IClock _clock;

        public RunDeployment(
            DeploymentProcessSettings deploymentProcessSettings,
            DeploymentProcessRunner deploymentProcessRunner,
            DeploymentProcessDirectoryFactory deploymentProcessDirectoryFactory,
            IClock clock)
        {
            _deploymentProcessSettings = deploymentProcessSettings;
            _deploymentProcessRunner = deploymentProcessRunner;
            _deploymentProcessDirectoryFactory = deploymentProcessDirectoryFactory;
            _clock = clock;
        }

        public void Run(
            Project project,
            Package package,
            Deployment deployment,
            DeploymentProcess deploymentProcess)
        {
            try
            {
                // todo: extract part of it to another class
                var deploymentWorkingDir = _deploymentProcessSettings.WorkingDir
                    .AddSegment(project.Id)
                    .AddSegment("deploying-packages")
                    .AddSegment($"{_clock.UtcNow.FileFriendlyFormat()}_{deployment.EnvironmentId}_package-{package.Id}");

                // todo: make asynchronous
                var processDirectory = _deploymentProcessDirectoryFactory.CreateFromZip(
                    deploymentProcessZip: deploymentProcess.File,
                    toDirectory: deploymentWorkingDir
                );
                
                var processOutput = _deploymentProcessRunner.Run(
                    processDirectory.Path.ToString(),
                    deploymentProcessFile: processDirectory.OpenProcessDescriptionFile(),
                    variablesForEnvironment: package.GetVariablesForEnvironment(deployment.EnvironmentId)
                );

                deployment.MarkFinished(processOutput);
            }
            catch (Exception)
            {
                deployment.MarkExecutionFailed();

                throw;
            }
        }
    }
}