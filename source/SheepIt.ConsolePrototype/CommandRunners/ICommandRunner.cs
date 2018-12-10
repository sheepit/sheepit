using System.Collections.Generic;
using SheepIt.ConsolePrototype.ScriptFiles;

namespace SheepIt.ConsolePrototype.CommandRunners
{
    public interface ICommandRunner
    {
        CommandResult Run(string command, IEnumerable<Variable> variables, string workingDir);
    }
}