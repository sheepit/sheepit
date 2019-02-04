using System.Collections.Generic;
using SheepIt.Api.Core.DeploymentProcessRunning.DeploymentProcessAccess;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Releases;

namespace SheepIt.Api.Core.DeploymentProcessRunning.CommandRunning
{
    public class BashCommandRunner : ICommandRunner
    {
        private readonly DeploymentProcessSettings _settings;
        private readonly SystemProcessRunner _systemProcessRunner = new SystemProcessRunner();

        public BashCommandRunner(DeploymentProcessSettings settings)
        {
            _settings = settings;
        }
        
        public ProcessStepResult Run(string command, IEnumerable<VariableForEnvironment> variables)
        {
            var workingDir = _settings.WorkingDir.ToString();
            var bashPath = _settings.Shell.Bash.ToString();

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