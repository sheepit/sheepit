using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.CommandRunners;
using SheepIt.Api.Tests.TestInfrastructure;
using SheepIt.Domain;

namespace SheepIt.Api.Tests
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

            var result = RunCommand("echo %TEST%", new VariableForEnvironment[]
            {
                new VariableForEnvironment("TEST", variableValue)
            });

            result.Output[0].Trim().Should().Be(variableValue);
        }

        [Test]
        public void can_check_if_command_succeeded()
        {
            var result = RunCommand("echo test");

            result.Successful.Should().BeTrue();
        }

        [Test]
        public void can_check_if_command_failed()
        {
            var result = RunCommand("some_unknown_command");

            result.Successful.Should().BeFalse();
            result.Output.Should().NotBeEmpty();
        }

        private static ProcessStepResult RunCommand(string command)
        {
            return RunCommand(command, Enumerable.Empty<VariableForEnvironment>());
        }

        private static ProcessStepResult RunCommand(string command, IEnumerable<VariableForEnvironment> variables)
        {
            var config = TestConfigurationFactory.Build();
            config["WorkingDirectory"] = TestContext.CurrentContext.TestDirectory;

            var commandRunner = new CmdCommandRunner(config);

            return commandRunner.Run(command, variables);
        }
    }
}