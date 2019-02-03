using Microsoft.Extensions.Configuration;
using SheepIt.Api.Infrastructure;

namespace SheepIt.Api.Core.DeploymentProcessRunning.DeploymentProcessAccess
{
    public class DeploymentProcessSettings
    {
        public LocalPath WorkingDir { get; }

        public DeploymentProcessSettings(IConfiguration configuration)
        {
            var workingDirString = configuration.GetValue<string>("WorkingDir");

            WorkingDir = new LocalPath(workingDirString);
        }
    }
}