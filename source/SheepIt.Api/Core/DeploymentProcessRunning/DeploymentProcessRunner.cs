using System.Collections.Generic;
using System.Linq;
using SheepIt.Api.Core.DeploymentProcessRunning.CommandRunning;
using SheepIt.Api.Core.DeploymentProcessRunning.DeploymentProcessAccess;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Releases;

namespace SheepIt.Api.Core.DeploymentProcessRunning
{
    // todo: we should consider supporting powershell
    // todo: we should handle error output for all runners

    public class DeploymentProcessRunner
    {
        private readonly Dictionary<string, ICommandRunner> _commandRunners;

        public DeploymentProcessRunner(DeploymentProcessSettings deploymentProcessSettings)
        {
            _commandRunners = new Dictionary<string, ICommandRunner>
            {
                { "cmd", new CmdCommandRunner(deploymentProcessSettings) },
                { "bash", new BashCommandRunner(deploymentProcessSettings) }
            };
        }

        public ProcessOutput Run(DeploymentProcessFile deploymentProcessFile, VariableForEnvironment[] variablesForEnvironment)
        {
            var processStepResults = RunCommands(deploymentProcessFile, variablesForEnvironment);

            return new ProcessOutput
            {
                Steps = processStepResults.ToArray()
            };
        }

        private IEnumerable<ProcessStepResult> RunCommands(DeploymentProcessFile deploymentProcessFile, VariableForEnvironment[] variablesForEnvironment)
        {
            var commandRunner = _commandRunners[deploymentProcessFile.Shell];

            foreach (var command in deploymentProcessFile.Commands)
            {
                var commandResult = commandRunner.Run(command, variablesForEnvironment);

                yield return commandResult;
                
                if (!commandResult.Successful)
                {
                    yield break;
                }
            }
        }     
    }
}
