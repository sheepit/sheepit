using System.Collections.Generic;
using SheepIt.Domain;

namespace SheepIt.Api.CommandRunners
{
    public class BashCommandRunner : ICommandRunner
    {
        public ProcessStepResult Run(string command, IEnumerable<VariableForEnvironment> variables, string workingDir)
        {
            var systemProcessResult = SystemProcessRunner.RunProcess(
                workingDir: workingDir,
                fileName: @"C:\Program Files\Git\bin\bash.exe", // todo: obviously this shouldn't be hardcoded
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