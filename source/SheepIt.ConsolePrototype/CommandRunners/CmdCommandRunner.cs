using System.Collections.Generic;
using SheepIt.Domain;

namespace SheepIt.ConsolePrototype.CommandRunners
{
    public class CmdCommandRunner : ICommandRunner
    {
        public ProcessStepResult Run(string command, IEnumerable<VariableForEnvironment> variables, string workingDir)
        {
            var systemProcessResult = SystemProcessRunner.RunProcess(
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