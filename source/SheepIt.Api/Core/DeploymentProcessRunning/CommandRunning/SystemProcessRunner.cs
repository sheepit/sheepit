using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using SheepIt.Api.Core.Releases;

namespace SheepIt.Api.Core.DeploymentProcessRunning.CommandRunning
{
    public class SystemProcessRunner
    {
        public SystemProcessResult RunProcess(
            string workingDir,
            string fileName,
            string arguments,
            IEnumerable<VariableForEnvironment> variables,
            string standardInputOrNull = null)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                WorkingDirectory = workingDir,
                RedirectStandardInput = standardInputOrNull != null,

                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            foreach (var variable in variables)
            {
                processStartInfo.EnvironmentVariables[variable.Name] = variable.Value;
            }

            using (var process = new Process { StartInfo = processStartInfo })
            {
                var output = RunProcessAndGetOutput(process, standardInputOrNull);

                return new SystemProcessResult
                {
                    Successful = process.ExitCode == 0,
                    Output = output
                };
            }
        }

        public class SystemProcessResult
        {
            public string[] Output { get; set; }
            public bool Successful { get; set; }
        }

        private string[] RunProcessAndGetOutput(Process process, string standardInputOrNull)
        {
            // todo: exception handling

            var outputLines = new List<string>();
            var outputLinesLock = new object();

            using (var standardOutputAutoResetEvent = new AutoResetEvent(initialState: false))
            using (var standardErrorAutoResetEvent = new AutoResetEvent(initialState: false))
            {
                // todo: we should clear these event handlers before leaving scope

                process.OutputDataReceived += (sender, args) =>
                {
                    if (args.Data == null)
                    {
                        standardOutputAutoResetEvent.Set();
                    }
                    else
                    {
                        lock (outputLinesLock)
                        {
                            outputLines.Add(args.Data);
                        }
                    }
                };

                process.ErrorDataReceived += (sender, args) =>
                {
                    if (args.Data == null)
                    {
                        standardErrorAutoResetEvent.Set();
                    }
                    else
                    {
                        lock (outputLinesLock)
                        {
                            outputLines.Add(args.Data);
                        }
                    }
                };

                process.Start();

                if (standardInputOrNull != null)
                {
                    // we send command via standard input rather than by -c flag to handle quotes correctly
                    process.StandardInput.Write(standardInputOrNull); // todo: is this a correct time to send input?
                    process.StandardInput.Close(); // todo: should we close it? should we dispose it?                    
                }

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                standardOutputAutoResetEvent.WaitOne();
                standardErrorAutoResetEvent.WaitOne();
            }

            string[] output;

            lock (outputLinesLock)
            {
                output = outputLines.ToArray();
            }

            return output;
        }
    }
}