using System;
using Autofac;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace SheepIt.Api.Infrastructure.Web
{
    public class WebApp : IDisposable
    {
        private readonly ILifetimeScope _rootScope;
        private readonly IConfiguration _configuration;
        
        private IWebHost _webHost;

        public WebApp(ILifetimeScope rootScope, IConfiguration configuration)
        {
            _rootScope = rootScope;
            _configuration = configuration;
        }

        public void StartAndWait(string[] args)
        {
            _webHost = WebHost
                .CreateDefaultBuilder(args)
                .UseStartup<AutofacStartup>()
                .UseSerilog()
                .ConfigureServices(ConfigureServices)
                .Build();
            
            // use Start() for integration tests
            _webHost.Run();
        }

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddInstance(_rootScope);
            serviceCollection.AddInstance(new Startup());
            serviceCollection.ReplaceWithInstance(_configuration);
        }

        public void Dispose()
        {
            _webHost?.Dispose();
        }
    }
}