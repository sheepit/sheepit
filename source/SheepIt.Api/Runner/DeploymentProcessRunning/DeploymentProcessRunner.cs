using System.Linq;
using SheepIt.Api.Model.Deployments;
using SheepIt.Api.Model.Packages;
using SheepIt.Api.Runner.DeploymentProcessRunning.CommandsRunning;
using SheepIt.Api.Runner.DeploymentProcessRunning.DeploymentProcessAccess;

namespace SheepIt.Api.Runner.DeploymentProcessRunning
{
    // todo: we should handle error output for all runners

    public class DeploymentProcessRunner
    {
        private readonly DeploymentProcessSettings _deploymentProcessSettings;

        public DeploymentProcessRunner(DeploymentProcessSettings deploymentProcessSettings)
        {
            _deploymentProcessSettings = deploymentProcessSettings;
        }

        public ProcessOutput Run(string workingDir, DeploymentProcessFile deploymentProcessFile,
            VariableForEnvironment[] variablesForEnvironment)
        {
            var _commandsRunner = new BashCommandsRunner(
                workingDir,
                _deploymentProcessSettings.Shell.Bash.ToString()
            );
            
            var commandsOutput = _commandsRunner.Run(deploymentProcessFile.Commands, variablesForEnvironment);
            
            return new ProcessOutput
            {
                Steps = new []
                {
                    new ProcessStepResult
                    {
                        Command = "all commands",
                        Output = commandsOutput.Output.ToArray(),
                        Successful = commandsOutput.Successful
                    }
                }
            };
        }
    }
}
