using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.ConsolePrototype.CommandRunners;
using SheepIt.ConsolePrototype.ScriptFiles;

namespace SheepIt.ConsolePrototype.Tests
{
    public class CmdCommandRunnerTests
    {
        [Test]
        public void can_run_a_cmd_command()
        {
            // it's important to check if quotes work properly
            var result = RunCommand(@"dir ""c:\program files""");

            result.Output.Should().NotBeEmpty();

            Console.WriteLine(result);
        }

        [Test]
        public void can_inject_some_environmental_variables_into_the_process()
        {
            var variableValue = Guid.NewGuid().ToString();

            var result = RunCommand("echo %TEST%", new Variable[]
            {
                new Variable("TEST", variableValue)
            });

            result.Output.Trim().Should().Be(variableValue);
        }

        [Test]
        public void can_check_if_command_succeeded()
        {
            var result = RunCommand("echo test");

            result.WasSuccessful.Should().BeTrue();
        }

        [Test]
        public void can_check_if_command_failed()
        {
            var result = RunCommand("some_unknown_command");

            result.WasSuccessful.Should().BeFalse();
        }

        private static CommandResult RunCommand(string command)
        {
            return RunCommand(command, Enumerable.Empty<Variable>());
        }

        private static CommandResult RunCommand(string command, IEnumerable<Variable> variables)
        {
            var commandRunner = new CmdCommandRunner();

            return commandRunner.Run(command, variables, TestContext.CurrentContext.TestDirectory);
        }
    }
}