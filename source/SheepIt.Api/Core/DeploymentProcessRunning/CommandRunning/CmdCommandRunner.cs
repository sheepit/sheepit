using System.Collections.Generic;
using SheepIt.Api.Core.DeploymentProcessRunning.DeploymentProcessAccess;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Releases;

namespace SheepIt.Api.Core.DeploymentProcessRunning.CommandRunning
{
    public class CmdCommandRunner : ICommandRunner
    {
        private readonly DeploymentProcessSettings _settings;
        private readonly SystemProcessRunner _systemProcessRunner = new SystemProcessRunner();

        public CmdCommandRunner(DeploymentProcessSettings settings)
        {
            _settings = settings;
        }

        public ProcessStepResult Run(string command, IEnumerable<VariableForEnvironment> variables)
        {
            var workingDir = _settings.WorkingDir.ToString();
            var cmdPath = _settings.Shell.Cmd.ToString();

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