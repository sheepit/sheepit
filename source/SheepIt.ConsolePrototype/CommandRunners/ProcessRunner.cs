using System;
using System.Collections.Generic;

namespace SheepIt.ConsolePrototype.CommandRunners
{
    public class ProcessRunner
    {
        public void Run(ProcessFile processFile, Variable[] variables, string workingDir)
        {
            var commandRunner = _commandRunners[processFile.Shell];
            
            foreach (var command in processFile.Commands)
            {
                var commandResult = commandRunner.Run(command, variables, workingDir);

                if (!commandResult.WasSuccessful)
                {
                    throw new ApplicationException($"Deployment failed on following command: {command}");
                }

                Console.WriteLine(commandResult.Output);
                Console.WriteLine();
            }
        }

        private readonly Dictionary<string, ICommandRunner> _commandRunners = new Dictionary<string, ICommandRunner>
        {
            { "cmd", new CmdCommandRunner() },
            { "bash", new BashCommandRunner() }
        };
    }
}
