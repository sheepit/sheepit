using System;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Debugging;
using Serilog.Sinks.AwsCloudWatch;
using SheepIt.Api.Infrastructure.Logger.CloudWatch;

namespace SheepIt.Api.Infrastructure.Logger
{
    internal class LoggerFactory
    {
        public static ILogger CreateLogger(IConfiguration configuration)
        {
            SelfLog.Enable(Console.Error);

            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration);

            var awsConfig = configuration.GetAwsConfigSection();
            if (awsConfig?.CloudWatch?.Enabled == true)
            {
                loggerConfiguration = loggerConfiguration.WriteTo.AmazonCloudWatch(
                    AwsCloudWatchConfiguration.BuildCloudWatchSinkOptions(),
                    AwsCloudWatchConfiguration.BuildAmazonCloudWatchLogsClient(awsConfig)
                );    
            }

            return loggerConfiguration.CreateLogger();
        }

    }
}