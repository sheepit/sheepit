using System.Collections.Generic;
using SheepIt.Api.Core.DeploymentProcessRunning.DeploymentProcessAccess;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Releases;

namespace SheepIt.Api.Core.DeploymentProcessRunning.CommandRunning
{
    public class CmdCommandRunner : ICommandRunner
    {
        private readonly DeploymentProcessSettings _deploymentProcessSettings;
        private readonly ShellSettings _shellSettings;
        private readonly SystemProcessRunner _systemProcessRunner = new SystemProcessRunner();

        public CmdCommandRunner(DeploymentProcessSettings deploymentProcessSettings, ShellSettings shellSettings)
        {
            _deploymentProcessSettings = deploymentProcessSettings;
            _shellSettings = shellSettings;
        }

        public ProcessStepResult Run(string command, IEnumerable<VariableForEnvironment> variables)
        {
            var workingDir = _deploymentProcessSettings.WorkingDir.ToString();
            var cmdPath = _shellSettings.Cmd.ToString();

            var systemProcessResult = _systemProcessRunner.RunProcess(
                workingDir: workingDir,
                fileName: cmdPath,
                arguments: $"/s /c \"{command}\"", // /c parameter runs inline command, /s handles outermost quotes
                variables: variables
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