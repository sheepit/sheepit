using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace SheepIt.Api.Infrastructure.Web
{
    public class AutofacStartup
    {
        private readonly Startup _startup;
        
        private readonly ILifetimeScope _rootScope;
        
        private ILifetimeScope _webScope;
       
        public AutofacStartup(Startup startup, ILifetimeScope rootScope)
        {
            _rootScope = rootScope;
            _startup = startup;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            _startup.ConfigureServices(services);
            
            // web scope has to be created after all services are configured, so they are included
            _webScope = _rootScope.BeginLifetimeScope(builder =>
            {
                var servicesToRegister = services.WithoutTypes(typeof(ILifetimeScope));

                builder.Populate(servicesToRegister);
            });
            
            return new AutofacServiceProvider(_webScope);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            _startup.Configure(app, env);
            
            applicationLifetime.ApplicationStopped.Register(() =>
            {
                _webScope?.Dispose();
            });
        }
    }
}
