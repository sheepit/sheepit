using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SheepIt.ConsolePrototype.CommandRunners
{
    public class CmdCommandRunner
    {
        public string Run(string command, IEnumerable<Variable> variables)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/s /c \"{command}\"", // /c parameter runs inline command, /s handles outermost quotes
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

            return process.StandardOutput.ReadToEnd();
        }
    }
}