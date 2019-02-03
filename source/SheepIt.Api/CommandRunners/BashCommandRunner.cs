using System.Collections.Generic;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Releases;
using SheepIt.Api.Infrastructure;

namespace SheepIt.Api.CommandRunners
{
    public class BashCommandRunner : ICommandRunner
    {
        private readonly ProcessSettings _processSettings;
        private readonly ShellSettings _shellSettings;
        private readonly SystemProcessRunner _systemProcessRunner = new SystemProcessRunner();

        public BashCommandRunner(ProcessSettings processSettings, ShellSettings shellSettings)
        {
            _processSettings = processSettings;
            _shellSettings = shellSettings;
        }
        
        public ProcessStepResult Run(string command, IEnumerable<VariableForEnvironment> variables)
        {
            var workingDir = _processSettings.WorkingDir.ToString();
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