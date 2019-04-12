using Microsoft.Extensions.Configuration;
using SheepIt.Api.Infrastructure.Utils;

namespace SheepIt.Api.Infrastructure.ErrorHandling
{
    public class ErrorHandlingSettings
    {
        public bool DeveloperDetails { get; }

        public ErrorHandlingSettings(IConfiguration configuration)
        {
            var errorHandlingSection = configuration.GetSection("ErrorHandling");

            DeveloperDetails = errorHandlingSection["DeveloperDetails"].ToBool();
        }
    }
}