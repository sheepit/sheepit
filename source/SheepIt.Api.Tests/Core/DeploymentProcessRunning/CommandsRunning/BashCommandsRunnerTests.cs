using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Core.DeploymentProcessRunning.CommandsRunning;
using SheepIt.Api.Core.DeploymentProcessRunning.DeploymentProcessAccess;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Releases;
using SheepIt.Api.Infrastructure.Utils;
using SheepIt.Api.Tests.TestInfrastructure;

namespace SheepIt.Api.Tests.Core.DeploymentProcessRunning.CommandsRunning
{
    // todo: there is a lot of duplication between cmd and bash tests

    public class BashCommandsRunnerTests
    {
        [Test]
        public void can_run_a_command()
        {
            var result = RunCommand(@"echo command output");

            var stepResult = result.ShouldHaveSingleStepResult();

            stepResult.Output.Should().Equal(
                @"echo command output",
                "command output",
                ""
            );
        }

        [Test]
        public void can_run_and_print_a_command_with_quotes_and_special_characters()
        {
            var result = RunCommand(@"echo ""\\   $? `echo 1` 2""");

            var stepResult = result.ShouldHaveSingleStepResult();

            stepResult.Output.Should().Equal(
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

            result.ShouldHaveSingleStepResult()
                .Output
                .Should().Contain(variableValue);
        }

        [Test]
        public void can_check_if_command_succeeded()
        {
            var result = RunCommand("echo test");

            var stepResult = result.ShouldHaveSingleStepResult();
            
            stepResult.Successful.Should().BeTrue();
            stepResult.Output.Should().NotBeEmpty();
        }

        [Test]
        public void can_check_if_command_failed()
        {
            var result = RunCommand("some_unknown_command");

            var stepResult = result.ShouldHaveSingleStepResult();
            
            stepResult.Successful.Should().BeFalse();
            stepResult.Output.Should().NotBeEmpty();
        }

        [Test]
        public void can_run_multiple_commands()
        {
            var processOutput = RunCommand(
                "echo first command",
                "echo second command",
                "echo third command");

            var stepResult = processOutput.ShouldHaveSingleStepResult();
            
            stepResult.Output.Should().Equal(
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
            var processOutput = RunCommand(
                "echo before failure",
                "unknown_command",
                "echo after failure"
            );

            var stepResult = processOutput.ShouldHaveSingleStepResult();

            stepResult.Output.Should().StartWith(new[]
            {
                "echo before failure",
                "before failure",
                "",
                "unknown_command"
            });

            stepResult.Output.Should().NotContain("echo after failure");
            stepResult.Output.Should().NotContain("after failure");
        }

        private static ProcessOutput RunCommand(params string[] commands)
        {
            return RunCommand(commands, new VariableForEnvironment[0]);
        }

        private static ProcessOutput RunCommand(string[] commands, VariableForEnvironment[] variables)
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

        private static void PrintResults(ProcessOutput result)
        {
            foreach (var processStepResult in result.Steps)
            {
                var commandResultString = processStepResult.Successful ? "SUCCESS" : "FAILED";

                Console.WriteLine($"{processStepResult.Command} ({commandResultString})");

                foreach (var line in processStepResult.Output)
                {
                    Console.WriteLine($"    {line}");
                }

                Console.WriteLine();
            }
        }
    }

    public static class ProcessOutputAssertions
    {
        public static ProcessStepResult ShouldHaveSingleStepResult(this ProcessOutput processOutput)
        {
            processOutput.Steps.Should().HaveCount(1);

            return processOutput.Steps.Single();
        }
    }
}