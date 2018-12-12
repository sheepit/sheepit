using System.Collections.Generic;
using System.Diagnostics;
using SheepIt.ConsolePrototype.ScriptFiles;

namespace SheepIt.ConsolePrototype.CommandRunners
{
    public class CmdCommandRunner : ICommandRunner
    {
        public CommandResult Run(string command, IEnumerable<Variable> variables, string workingDir)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/s /c \"{command}\"", // /c parameter runs inline command, /s handles outermost quotes
                WorkingDirectory = workingDir,
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

            var output = process.StandardOutput.ReadLinesToEnd();

            return new CommandResult
            {
                Output = output,
                WasSuccessful = process.ExitCode == 0
            };
        }
    }

    public class CommandResult
    {
        public bool WasSuccessful { get; set; }
        public string[] Output { get; set; }
    }
}