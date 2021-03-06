﻿using Microsoft.Extensions.Configuration;
using SheepIt.Api.Infrastructure;

namespace SheepIt.Api.Runner.DeploymentProcessRunning.DeploymentProcessAccess
{
    public class DeploymentProcessSettings
    {
        public LocalPath WorkingDir { get; }
        public ShellSettings Shell { get; }

        public DeploymentProcessSettings(IConfiguration configuration)
        {
            var workingDirString = configuration.GetValue<string>("DeploymentProcess:WorkingDir");
            
            WorkingDir = new LocalPath(workingDirString);
            Shell = new ShellSettings(configuration);
        }

        public class ShellSettings
        {
            // todo: should be called BashPath/Command?
            public LocalPath Bash { get; }

            public ShellSettings(IConfiguration configuration)
            {
                var shell = configuration.GetSection("DeploymentProcess:Shell");

                Bash = new LocalPath(shell["Bash"]);
            }
        }
    }
}