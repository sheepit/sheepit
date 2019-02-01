using System.Collections.Generic;
using SheepIt.Api.Infrastructure;
using SheepIt.Domain;

namespace SheepIt.Api.CommandRunners
{
    public class CmdCommandRunner : ICommandRunner
    {
        private readonly ProcessSettings _processSettings;
        private readonly ShellSettings _shellSettings;
        private readonly SystemProcessRunner _systemProcessRunner = new SystemProcessRunner();

        public CmdCommandRunner(ProcessSettings processSettings, ShellSettings shellSettings)
        {
            _processSettings = processSettings;
            _shellSettings = shellSettings;
        }

        public ProcessStepResult Run(string command, IEnumerable<VariableForEnvironment> variables)
        {
            var workingDir = _processSettings.WorkingDir.ToString();
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