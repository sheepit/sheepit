using System.Collections.Generic;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Releases;

namespace SheepIt.Api.Core.DeploymentProcessRunning.CommandRunning
{
    public interface ICommandRunner
    {
        ProcessStepResult Run(string command, IEnumerable<VariableForEnvironment> variables);
    }
}