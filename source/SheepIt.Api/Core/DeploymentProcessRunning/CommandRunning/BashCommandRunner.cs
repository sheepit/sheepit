using System.Collections.Generic;
using SheepIt.Api.Core.DeploymentProcessRunning.DeploymentProcessAccess;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Releases;

namespace SheepIt.Api.Core.DeploymentProcessRunning.CommandRunning
{
    public class BashCommandRunner : ICommandRunner
    {
        private readonly DeploymentProcessSettings _deploymentProcessSettings;
        private readonly ShellSettings _shellSettings;
        private readonly SystemProcessRunner _systemProcessRunner = new SystemProcessRunner();

        public BashCommandRunner(DeploymentProcessSettings deploymentProcessSettings, ShellSettings shellSettings)
        {
            _deploymentProcessSettings = deploymentProcessSettings;
            _shellSettings = shellSettings;
        }
        
        public ProcessStepResult Run(string command, IEnumerable<VariableForEnvironment> variables)
        {
            var workingDir = _deploymentProcessSettings.WorkingDir.ToString();
            var bashPath = _shellSettings.Bash.ToString();

            var systemProcessResult = _systemProcessRunner.RunProcess(
                workingDir: workingDir,
                fileName: bashPath,
                arguments: "-s", // -s will read command from standard input
                variables: variables,
                standardInputOrNull: command
            );
            
            return new ProcessStepResult
            {
                Command = command,
                Successful = systemProcessResult.Successful,
                Output = systemProcessResult.Output
            };
        }
    }
}