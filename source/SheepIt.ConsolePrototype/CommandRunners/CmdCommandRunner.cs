using System.Collections.Generic;
using System.Diagnostics;
using SheepIt.ConsolePrototype.ScriptFiles;
using SheepIt.Domain;

namespace SheepIt.ConsolePrototype.CommandRunners
{
    public class CmdCommandRunner : ICommandRunner
    {
        public ProcessStepResult Run(string command, IEnumerable<VariableForEnvironment> variables, string workingDir)
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

            return new ProcessStepResult
            {
                Command = command,
                Successful = process.ExitCode == 0,
                Output = output,
            };
        }
    }
}