using System.Collections.Generic;
using SheepIt.ConsolePrototype.ScriptFiles;
using SheepIt.Domain;

namespace SheepIt.ConsolePrototype.CommandRunners
{
    public interface ICommandRunner
    {
        ProcessStepResult Run(string command, IEnumerable<VariableForEnvironment> variables, string workingDir);
    }
}