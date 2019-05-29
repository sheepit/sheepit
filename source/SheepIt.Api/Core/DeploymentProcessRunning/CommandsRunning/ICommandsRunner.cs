using System.Collections.Generic;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Releases;

namespace SheepIt.Api.Core.DeploymentProcessRunning.CommandsRunning
{
    public interface ICommandsRunner
    {
        ProcessOutput Run(
            IReadOnlyList<string> commands,
            IReadOnlyList<VariableForEnvironment> variables);
    }
}