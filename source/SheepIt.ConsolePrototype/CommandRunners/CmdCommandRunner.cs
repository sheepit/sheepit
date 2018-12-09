using System.Diagnostics;

namespace SheepIt.ConsolePrototype.CommandRunners
{
    public class CmdCommandRunner
    {
        public string Run(string command)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/s /c \"{command}\"", // /c parameter runs inline command, /s handles outermost quotes
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            process.Start();

            return process.StandardOutput.ReadToEnd();
        }
    }
}