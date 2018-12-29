using System;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Debugging;

namespace SheepIt.Api.Infrastructure.Logger
{
    internal class LoggerFactory
    {
        public static ILogger CreateLogger(IConfiguration configuration)
        {
            SelfLog.Enable(Console.Error);

            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration);

            return loggerConfiguration.CreateLogger();
        }

    }
}