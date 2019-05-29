using System;
using System.Collections.Generic;
using System.Linq;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Releases;
using SheepIt.Api.Infrastructure.Utils;

namespace SheepIt.Api.Core.DeploymentProcessRunning.CommandsRunning
{
    public class BashCommandsRunner : ICommandsRunner
    {
        private readonly string _workingDir;
        private readonly string _bashPath;
        
        private readonly SystemProcessRunner _systemProcessRunner = new SystemProcessRunner();

        public BashCommandsRunner(string workingDir, string bashPath)
        {
            _workingDir = workingDir;
            _bashPath = bashPath;
        }

        public ProcessOutput Run(
            IReadOnlyList<string> commands,
            IReadOnlyList<VariableForEnvironment> variables)
        {
            var decoratedCommands = commands
                .SelectMany(command => new[]
                {
                    EchoCommand(command), 
                    command,
                    EchoNewLine()
                })
                .ToArray();

            // "set -e" will fail on first error
            // also, watch for gotchas: http://mywiki.wooledge.org/BashFAQ/105
            var allCommands = new[] {"set -e"}
                .Concat(decoratedCommands);
            
            var joinedCommands = allCommands.JoinWith("\n");

            var processResult = RunBashProcess(variables, joinedCommands);

            var processStepResult = new ProcessStepResult
            {
                Command = "all commands",
                Output = processResult.Output,
                Successful = processResult.Successful
            };

            return new ProcessOutput
            {
                Steps = new[] { processStepResult }
            };
        }

        private SystemProcessRunner.SystemProcessResult RunBashProcess(
            IReadOnlyList<VariableForEnvironment> variables,
            string command)
        {
            return _systemProcessRunner.RunProcess(
                workingDir: _workingDir,
                fileName: _bashPath,
                arguments: "-s", // -s will read command from standard input
                variables: variables,
                standardInputOrNull: command
            );
        }
        
        private readonly string[] BashSpecialCharacters = { "\\", "\"", "$", "`" };

        private string EchoCommand(string command)
        {
            var escapedCommand = command;
            
            foreach (var specialCharacter in BashSpecialCharacters)
            {
                escapedCommand = escapedCommand.Replace(
                    specialCharacter,
                    $"\\{specialCharacter}"
                );
            }

            return $"echo \"{escapedCommand}\"";
        }

        private string EchoNewLine()
        {
            return "echo";
        }
    }
}