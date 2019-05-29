using System.Collections.Generic;
using System.Linq;
using SheepIt.Api.Core.DeploymentProcessRunning.CommandsRunning;
using SheepIt.Api.Core.DeploymentProcessRunning.DeploymentProcessAccess;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Releases;

namespace SheepIt.Api.Core.DeploymentProcessRunning
{
    // todo: we should handle error output for all runners

    public class DeploymentProcessRunner
    {
        private readonly ICommandsRunner _commandsRunner;

        public DeploymentProcessRunner(DeploymentProcessSettings deploymentProcessSettings)
        {
            // todo: inject
            _commandsRunner = new BashCommandsRunner(
                deploymentProcessSettings.WorkingDir.ToString(),
                deploymentProcessSettings.Shell.Bash.ToString()
            );
        }

        public ProcessOutput Run(DeploymentProcessFile deploymentProcessFile, VariableForEnvironment[] variablesForEnvironment)
        {
            return _commandsRunner.Run(deploymentProcessFile.Commands, variablesForEnvironment);
        }
    }
}
