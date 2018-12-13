using System.Collections.Generic;
using System.Diagnostics;
using SheepIt.ConsolePrototype.ScriptFiles;
using SheepIt.Domain;

namespace SheepIt.ConsolePrototype.CommandRunners
{
    public class BashCommandRunner : ICommandRunner
    {
        public CommandResult Run(string command, IEnumerable<Variable> variables, string workingDir)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = @"C:\Program Files\Git\bin\bash.exe", // todo: obviously this shouldn't be hardcoded
                Arguments = "-s", // -s will run command from standard input
                WorkingDirectory = workingDir,
                RedirectStandardInput = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            foreach (var variable in variables)
            {
                processStartInfo.EnvironmentVariables[variable.Name] = variable.Value;
            }

            var process = new Process
            {
                StartInfo = processStartInfo
            };

            process.Start();
            
            // we send command via standard input rather than by -c flag to handle quotes correctly
            process.StandardInput.Write(command);

            process.StandardInput.Close(); // todo: should we close it? should we dispose it?

            var output = process.StandardOutput.ReadLinesToEnd();

            return new CommandResult
            {
                Output = output,
                WasSuccessful = process.ExitCode == 0
            };
        }
    }
}