using System.Collections.Generic;
using SheepIt.Api.Core.Releases;

namespace SheepIt.Api.Core.DeploymentProcessRunning.CommandsRunning
{
    public interface ICommandsRunner
    {
        CommandsOutput Run(
            IReadOnlyList<string> commands,
            IReadOnlyList<VariableForEnvironment> variables);
    }

    public class CommandsOutput
    {
        public IReadOnlyList<string> Output { get; }
        public bool Successful { get; }

        public CommandsOutput(IReadOnlyList<string> output, bool successful)
        {
            Output = output;
            Successful = successful;
        }
    }
}