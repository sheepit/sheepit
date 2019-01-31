using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using SheepIt.Api.ScriptFiles;
using SheepIt.Domain;

namespace SheepIt.Api.CommandRunners
{
    // todo: we should consider supporting powershell
    // todo: we should handle error output for all runners

    public class ProcessRunner
    {
        private readonly IConfiguration _configuration;

        public ProcessRunner(IConfiguration configuration)
        {
            _configuration = configuration;
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
            var shells = _configuration.GetSection("Shell").Get<Dictionary<string, string>>();
            var shellPath = shells[processFile.Shell];

            foreach (var command in processFile.Commands)
            {
                var commandResult = commandRunner.Run(command, variablesForEnvironment, workingDir, shellPath);

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
