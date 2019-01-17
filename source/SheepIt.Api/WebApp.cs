using System;
using Autofac;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;

namespace SheepIt.Api
{
    public class WebApp : IDisposable
    {
        private readonly ILifetimeScope _rootScope;
        
        private IWebHost _webHost;

        public WebApp(ILifetimeScope rootScope)
        {
            _rootScope = rootScope;
        }

        public void Run(string[] args)
        {
            _webHost = WebHost
                .CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog()
                .ConfigureServices(ConfigureServices)
                .Build();
            
            // use start for integration tests
            _webHost.Run();
        }

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.Replace(new ServiceDescriptor(typeof(ILifetimeScope), _rootScope));
        }

        public void Dispose()
        {
            _webHost?.Dispose();
        }
    }
}