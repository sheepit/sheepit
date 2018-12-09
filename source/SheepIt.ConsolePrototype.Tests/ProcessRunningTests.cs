using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.ConsolePrototype.CommandRunners;

namespace SheepIt.ConsolePrototype.Tests
{
    public class ProcessRunningTests
    {
        [Test]
        public void can_run_a_cmd_command()
        {
            var commandRunner = new CmdCommandRunner();

            // it's important to check if quotes work properly
            var output = commandRunner.Run(@"dir ""c:\program files""", Enumerable.Empty<Variable>());

            output.Should().NotBeEmpty();

            Console.WriteLine(output);
        }

        [Test]
        public void can_inject_some_environmental_variables_into_the_process()
        {
            var commandRunner = new CmdCommandRunner();

            var variableValue = Guid.NewGuid().ToString();

            var output = commandRunner.Run("echo %TEST%", new Variable[]
            {
                new Variable("TEST", variableValue)
            });

            output.Trim().Should().Be(variableValue);
        }
    }
}