using System;
using System.Collections.Generic;
using System.Linq;
using SheepIt.ConsolePrototype.ScriptFiles;

namespace SheepIt.ConsolePrototype.CommandRunners
{
    // todo: we should consider supporting powershell
    // todo: we should handle error output for all runners

    public class ProcessResult
    {
        public ProcessStepResult[] Steps{ get; set; }
    }

    public class ProcessStepResult
    {
        public string Command { get; set; }
        public string[] Output { get; set; }
    }

    public class ProcessRunner
    {
        public ProcessResult Run(ProcessFile processFile, Variable[] variables, string workingDir)
        {
            var processStepResults = NewMethod(processFile, variables, workingDir);

            return new ProcessResult
            {
                Steps = processStepResults.ToArray()
            };
        }

        private IEnumerable<ProcessStepResult> NewMethod(ProcessFile processFile, Variable[] variables, string workingDir)
        {
            var commandRunner = _commandRunners[processFile.Shell];

            foreach (var command in processFile.Commands)
            {
                var commandResult = commandRunner.Run(command, variables, workingDir);

                if (!commandResult.WasSuccessful)
                {
                    throw new ApplicationException($"Deployment failed on following command: {command}");
                }

                yield return new ProcessStepResult
                {
                    Command = command,
                    Output = commandResult.Output
                };
            }
        }

        private readonly Dictionary<string, ICommandRunner> _commandRunners = new Dictionary<string, ICommandRunner>
        {
            { "cmd", new CmdCommandRunner() },
            { "bash", new BashCommandRunner() }
        };
    }
}
