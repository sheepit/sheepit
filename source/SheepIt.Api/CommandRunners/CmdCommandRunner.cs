using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using SheepIt.Domain;

namespace SheepIt.Api.CommandRunners
{
    public class CmdCommandRunner : ICommandRunner
    {
        private readonly IConfiguration _configuration;
        private readonly SystemProcessRunner _systemProcessRunner = new SystemProcessRunner();

        public CmdCommandRunner(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ProcessStepResult Run(string command, IEnumerable<VariableForEnvironment> variables)
        {
            var workingDir = _configuration["WorkingDirectory"];
            var cmdPath = _configuration["Shell:Cmd"];

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