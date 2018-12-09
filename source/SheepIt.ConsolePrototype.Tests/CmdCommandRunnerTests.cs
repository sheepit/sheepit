using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.ConsolePrototype.CommandRunners;

namespace SheepIt.ConsolePrototype.Tests
{
    public class CmdCommandRunnerTests
    {
        [Test]
        public void can_run_a_cmd_command()
        {
            var commandRunner = new CmdCommandRunner();

            // it's important to check if quotes work properly
            var result = commandRunner.Run(@"dir ""c:\program files""", Enumerable.Empty<Variable>());

            result.Output.Should().NotBeEmpty();

            Console.WriteLine(result);
        }

        [Test]
        public void can_inject_some_environmental_variables_into_the_process()
        {
            var commandRunner = new CmdCommandRunner();

            var variableValue = Guid.NewGuid().ToString();

            var result = commandRunner.Run("echo %TEST%", new Variable[]
            {
                new Variable("TEST", variableValue)
            });

            result.Output.Trim().Should().Be(variableValue);
        }

        [Test]
        public void can_check_if_command_succeeded()
        {
            var commandRunner = new CmdCommandRunner();

            var result = commandRunner.Run("echo test", Enumerable.Empty<Variable>());

            result.WasSuccessful.Should().BeTrue();
        }

        [Test]
        public void can_check_if_command_failed()
        {
            var commandRunner = new CmdCommandRunner();

            var result = commandRunner.Run("some_unknown_command", Enumerable.Empty<Variable>());

            result.WasSuccessful.Should().BeFalse();
        }
    }
}