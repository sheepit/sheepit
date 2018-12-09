using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.ConsolePrototype.CommandRunners;

namespace SheepIt.ConsolePrototype.Tests
{
    // todo: there is a lot of duplication between cmd and bash tests

    public class BashCommandRunnerTests
    {
        [Test]
        public void can_run_a_cmd_command()
        {
            var commandRunner = new BashCommandRunner();

            // it's important to check if quotes work properly
            var result = commandRunner.Run(@"ls ""/c/Program Files""", Enumerable.Empty<Variable>());

            result.Output.Should().NotBeEmpty();

            Console.WriteLine(result);
        }

        [Test]
        public void can_inject_some_environmental_variables_into_the_process()
        {
            var commandRunner = new BashCommandRunner();

            var variableValue = Guid.NewGuid().ToString();

            var result = commandRunner.Run("echo $TEST", new Variable[]
            {
                new Variable("TEST", variableValue)
            });

            result.Output.Trim().Should().Be(variableValue);
        }

        [Test]
        public void can_check_if_command_succeeded()
        {
            var commandRunner = new BashCommandRunner();

            var result = commandRunner.Run("echo test", Enumerable.Empty<Variable>());

            result.WasSuccessful.Should().BeTrue();
        }

        [Test]
        public void can_check_if_command_failed()
        {
            var commandRunner = new BashCommandRunner();

            var result = commandRunner.Run("some_unknown_command", Enumerable.Empty<Variable>());

            result.WasSuccessful.Should().BeFalse();
        }
    }
}