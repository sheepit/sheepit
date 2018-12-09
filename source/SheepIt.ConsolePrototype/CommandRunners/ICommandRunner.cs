using System.Collections.Generic;

namespace SheepIt.ConsolePrototype.CommandRunners
{
    public interface ICommandRunner
    {
        CommandResult Run(string command, IEnumerable<Variable> variables, string workingDir);
    }
}