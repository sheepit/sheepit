using System.Collections.Generic;
using System.Linq;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Releases;
using SheepIt.Api.Infrastructure;
using SheepIt.Api.ScriptFiles;

namespace SheepIt.Api.CommandRunners
{
    // todo: we should consider supporting powershell
    // todo: we should handle error output for all runners

    public class ProcessRunner
    {
        private readonly Dictionary<string, ICommandRunner> _commandRunners;

        public ProcessRunner(ProcessSettings processSettings, ShellSettings shellSettings)
        {
            _commandRunners = new Dictionary<string, ICommandRunner>
            {
                { "cmd", new CmdCommandRunner(processSettings, shellSettings) },
                { "bash", new BashCommandRunner(processSettings, shellSettings) }
            };
        }

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
                var commandResult = commandRunner.Run(command, variablesForEnvironment);

                yield return commandResult;
                
                if (!commandResult.Successful)
                {
                    yield break;
                }
            }
        }     
    }
}
