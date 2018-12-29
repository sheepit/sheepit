using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using SheepIt.Api.Infrastructure;
using SheepIt.Api.Infrastructure.Logger;

namespace SheepIt.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = ConfigurationFactory.CreateConfiguration();
            Log.Logger = LoggerFactory.CreateLogger(configuration);
            
            try
            {
                Log.Information("Starting web host");
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString(), "Terminated unexpectedly!");
                Log.Fatal(e, "Terminated unexpectedly!");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog();
    }
}
