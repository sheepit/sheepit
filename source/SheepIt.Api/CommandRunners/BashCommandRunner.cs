using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using SheepIt.Domain;

namespace SheepIt.Api.CommandRunners
{
    public class BashCommandRunner : ICommandRunner
    {
        private readonly IConfiguration _configuration;
        private readonly SystemProcessRunner _systemProcessRunner = new SystemProcessRunner();

        public BashCommandRunner(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public ProcessStepResult Run(string command, IEnumerable<VariableForEnvironment> variables)
        {
            var workingDir = _configuration["WorkingDirectory"];
            var bashPath = _configuration["Shell:Bash"];

            var systemProcessResult = _systemProcessRunner.RunProcess(
                workingDir: workingDir,
                fileName: bashPath,
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