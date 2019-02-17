﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Core.DeploymentProcessRunning.CommandRunning;
using SheepIt.Api.Core.DeploymentProcessRunning.DeploymentProcessAccess;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Releases;
using SheepIt.Api.Tests.TestInfrastructure;

namespace SheepIt.Api.Tests.Core.DeploymentProcessRunning.CommandRunning
{
    // todo: there is a lot of duplication between cmd and bash tests

    public class BashCommandRunnerTests
    {
        [Test]
        public void can_run_a_cmd_command()
        {
            // it's important to check if quotes work properly
            var result = RunCommand(@"ls ""/c/Program Files""");

            result.Output.Should().NotBeEmpty();

            Console.WriteLine(result);
        }

        [Test]
        public void can_inject_some_environmental_variables_into_the_process()
        {
            var variableValue = Guid.NewGuid().ToString();

            var result = RunCommand("echo $TEST", new VariableForEnvironment[]
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

            var processSettings = new DeploymentProcessSettings(config);
            
            var commandRunner = new BashCommandRunner(processSettings);

            return commandRunner.Run(command, variables);
        }
    }
}