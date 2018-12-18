using System.Collections.Generic;
using System.Diagnostics;
using SheepIt.ConsolePrototype.ScriptFiles;
using SheepIt.Domain;

namespace SheepIt.ConsolePrototype.CommandRunners
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