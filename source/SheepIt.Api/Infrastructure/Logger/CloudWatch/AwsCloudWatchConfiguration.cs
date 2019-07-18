using System;
using Amazon;
using Amazon.CloudWatchLogs;
using Serilog.Events;
using Serilog.Sinks.AwsCloudWatch;

namespace SheepIt.Api.Infrastructure.Logger.CloudWatch
{
    internal class AwsCloudWatchConfiguration
    {
        public static ICloudWatchSinkOptions BuildCloudWatchSinkOptions()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            return new CloudWatchSinkOptions
            {
                LogGroupName = $"sheepit/{environment}",
                CreateLogGroup = true,
                MinimumLogEventLevel = LogEventLevel.Debug,
                TextFormatter = new JsonTextFormatter()
            };
        }

        public static IAmazonCloudWatchLogs BuildAmazonCloudWatchLogsClient(
            AwsConfigSection awsConfigSection)
        {
            var region = awsConfigSection.Region;

            return new AmazonCloudWatchLogsClient(RegionEndpoint.GetBySystemName(region));
        }
    }
}
