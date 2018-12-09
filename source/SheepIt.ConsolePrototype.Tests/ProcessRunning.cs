using System;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.ConsolePrototype.CommandRunners;

namespace SheepIt.ConsolePrototype.Tests
{
    public class ProcessRunning
    {
        [Test]
        public void can_run_a_cmd_command()
        {
            var commandRunner = new CmdCommandRunner();

            // it's important to check if quotes work properly
            var output = commandRunner.Run(@"dir ""c:\program files""");

            output.Should().NotBeEmpty();

            Console.WriteLine(output);
        }
    }
}