using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SheepIt.Api.Infrastructure.Api;
using SheepIt.Api.Infrastructure.Logger;
using Swashbuckle.AspNetCore.Swagger;

namespace SheepIt.Api.Infrastructure.Web
{
    public class Startup
    {
        private readonly ILifetimeScope _rootScope;
        
        private ILifetimeScope _webScope;
        
        public Startup(ILifetimeScope rootScope)
        {
            _rootScope = rootScope;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            
            _webScope = _rootScope.BeginLifetimeScope(builder =>
            {
                var servicesToRegister = services.WithoutTypes(typeof(ILifetimeScope));

                builder.Populate(servicesToRegister);
            });
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "sheepIt", Version = "v1" });
            });
            
            return new AutofacServiceProvider(_webScope);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseDefaultFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true
            });

            app.UseMiddleware<SerilogMiddleware>();

            app.UseMvc();
            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            
            applicationLifetime.ApplicationStopped.Register(() =>
            {
                _webScope?.Dispose();
            });
        }
    }
}
