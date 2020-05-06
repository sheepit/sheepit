using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Infrastructure.Utils;
using SheepIt.Api.Model.Packages;
using SheepIt.Api.Runner.DeploymentProcessRunning.CommandsRunning;
using SheepIt.Api.Runner.DeploymentProcessRunning.DeploymentProcessAccess;
using SheepIt.Api.Tests.TestInfrastructure;

namespace SheepIt.Api.Tests.Core.DeploymentProcessRunning.CommandsRunning
{
    public class BashCommandsRunnerTests
    {
        [Test]
        public void can_run_a_command()
        {
            var result = RunCommand(@"echo command output");

            result.Output.Should().Equal(
                @"echo command output",
                "command output",
                ""
            );
        }

        [Test]
        public void can_run_and_print_a_command_with_quotes_and_special_characters()
        {
            var result = RunCommand(@"echo ""\\   $? `echo 1` 2""");

            result.Output.Should().Equal(
                @"echo ""\\   $? `echo 1` 2""",
                @"\   0 1 2",
                ""
            );
        }

        [Test]
        public void can_inject_some_environmental_variables_into_the_process()
        {
            var variableValue = Guid.NewGuid().ToString();

            var result = RunCommand(
                commands: new[]
                {
                    "echo $TEST"
                },
                variables: new VariableForEnvironment[]
                {
                    new VariableForEnvironment("TEST", variableValue)
                }
            );

            result.Output.Should().Contain(variableValue);
        }

        [Test]
        public void can_check_if_command_succeeded()
        {
            var result = RunCommand("echo test");

            result.Successful.Should().BeTrue();
            result.Output.Should().NotBeEmpty();
        }

        [Test]
        public void can_check_if_command_failed()
        {
            var result = RunCommand("some_unknown_command");

            result.Successful.Should().BeFalse();
            result.Output.Should().NotBeEmpty();
        }

        [Test]
        public void can_run_multiple_commands()
        {
            var result = RunCommand(
                "echo first command",
                "echo second command",
                "echo third command");

            result.Output.Should().Equal(
                "echo first command",
                "first command",
                "",
                "echo second command",
                "second command",
                "",
                "echo third command",
                "third command",
                ""
            );
        }

        [Test]
        public void will_stop_after_first_command_failed()
        {
            var result = RunCommand(
                "echo before failure",
                "unknown_command",
                "echo after failure"
            );

            result.Output.Should().StartWith(new[]
            {
                "echo before failure",
                "before failure",
                "",
                "unknown_command"
            });

            result.Output.Should().NotContain("echo after failure");
            result.Output.Should().NotContain("after failure");
        }

        [Test]
        public void commands_retain_directory_context()
        {
            if (Directory.Exists("foo"))
            {
                Directory.Delete("foo");
            }

            var result = RunCommand(
                "mkdir foo",
                "cd foo",
                "basename `pwd`"
            );

            result.Output.Should().Equal(
                "mkdir foo",
                "",
                "cd foo",
                "",
                "basename `pwd`",
                "foo",
                ""
            );
        }

        private static CommandsOutput RunCommand(params string[] commands)
        {
            return RunCommand(commands, new VariableForEnvironment[0]);
        }

        private static CommandsOutput RunCommand(string[] commands, VariableForEnvironment[] variables)
        {
            var config = TestConfigurationFactory.Build();

            var processSettings = new DeploymentProcessSettings(config);
            
            var commandRunner = new BashCommandsRunner(
                workingDir: TestContext.CurrentContext.TestDirectory,
                bashPath: processSettings.Shell.Bash.ToString()
            );
            
            var processOutput = commandRunner.Run(commands, variables);

            PrintResults(processOutput);

            return processOutput;
        }

        private static void PrintResults(CommandsOutput result)
        {
            foreach (var line in result.Output)
            {
                Console.WriteLine($"{line}");
            }

            Console.WriteLine();
            Console.WriteLine(result.Successful ? "SUCCESS" : "FAILED");
        }
    }
}