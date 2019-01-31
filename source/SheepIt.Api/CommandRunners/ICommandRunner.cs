using System.Collections.Generic;
using SheepIt.Domain;

namespace SheepIt.Api.CommandRunners
{
    public interface ICommandRunner
    {
        ProcessStepResult Run(string command, IEnumerable<VariableForEnvironment> variables, string workingDir, string shellPath);
    }
}