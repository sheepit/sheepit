using System.Collections.Generic;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Releases;

namespace SheepIt.Api.CommandRunners
{
    public interface ICommandRunner
    {
        ProcessStepResult Run(string command, IEnumerable<VariableForEnvironment> variables);
    }
}