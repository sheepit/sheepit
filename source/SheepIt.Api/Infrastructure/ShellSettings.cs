using Microsoft.Extensions.Configuration;

namespace SheepIt.Api.Infrastructure
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