using Microsoft.Extensions.Configuration;

namespace SheepIt.Api.Infrastructure
{
    public class ProcessSettings
    {
        public LocalPath WorkingDir { get; }

        public ProcessSettings(IConfiguration configuration)
        {
            var workingDirString = configuration.GetValue<string>("WorkingDir");

            WorkingDir = new LocalPath(workingDirString);
        }
    }
}