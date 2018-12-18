using System;
using System.Collections.Generic;
using System.Linq;
using SheepIt.ConsolePrototype.ScriptFiles;
using SheepIt.Domain;

namespace SheepIt.ConsolePrototype.CommandRunners
{
    // todo: we should consider supporting powershell
    // todo: we should handle error output for all runners

    public class ProcessRunner
    {
        public ProcessOutput Run(ProcessFile processFile, VariableForEnvironment[] variablesForEnvironment, string workingDir)
        {
            var processStepResults = NewMethod(processFile, variablesForEnvironment, workingDir);

            return new ProcessOutput
            {
                Steps = processStepResults.ToArray()
            };
        }

        private IEnumerable<ProcessStepResult> NewMethod(ProcessFile processFile, VariableForEnvironment[] variablesForEnvironment, string workingDir)
        {
            var commandRunner = _commandRunners[processFile.Shell];

            foreach (var command in processFile.Commands)
            {
                var commandResult = commandRunner.Run(command, variablesForEnvironment, workingDir);

                yield return commandResult;
                
                if (!commandResult.Successful)
                {
                    yield break;
                }
            }
        }

        private readonly Dictionary<string, ICommandRunner> _commandRunners = new Dictionary<string, ICommandRunner>
        {
            { "cmd", new CmdCommandRunner() },
            { "bash", new BashCommandRunner() }
        };
    }
}
