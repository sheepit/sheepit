using System.Collections.Generic;
using SheepIt.Domain;

namespace SheepIt.Api.CommandRunners
{
    public class CmdCommandRunner : ICommandRunner
    {
        private readonly SystemProcessRunner _systemProcessRunner = new SystemProcessRunner();
        
        public ProcessStepResult Run(string command, IEnumerable<VariableForEnvironment> variables, string workingDir, string shellPath)
        {
            var systemProcessResult = _systemProcessRunner.RunProcess(
                workingDir: workingDir,
                fileName: "cmd.exe",
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