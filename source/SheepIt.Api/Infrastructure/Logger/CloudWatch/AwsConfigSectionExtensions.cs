using Microsoft.Extensions.Configuration;

namespace SheepIt.Api.Infrastructure.Logger.CloudWatch
{
    internal static class AwsConfigSectionExtensions
    {
        public static AwsConfigSection GetAwsConfigSection(
            this IConfiguration configuration)
        {
            return configuration
                .GetSection("AWS")
                .Get<AwsConfigSection>();
        }
    }
}