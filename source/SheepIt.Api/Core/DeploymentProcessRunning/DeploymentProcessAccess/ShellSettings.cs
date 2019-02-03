using Microsoft.Extensions.Configuration;
using SheepIt.Api.Infrastructure;

namespace SheepIt.Api.Core.DeploymentProcessRunning.DeploymentProcessAccess
{
    public class ShellSettings
    {
        public LocalPath Bash { get; }
        public LocalPath Cmd { get; }

        public ShellSettings(IConfiguration configuration)
        {
            var shell = configuration.GetSection("Shell");

            Bash = new LocalPath(shell["Bash"]);
            Cmd = new LocalPath(shell["Cmd"]);
        }
    }
}