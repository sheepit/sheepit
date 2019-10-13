using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SheepIt.Api.Infrastructure.Authorization;
using SheepIt.Api.Infrastructure.ErrorHandling;
using SheepIt.Api.Infrastructure.Logger;
using SheepIt.Api.Infrastructure.Web;
using Swashbuckle.AspNetCore.Swagger;

namespace SheepIt.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            
            services.AddSheepItAuthentication(_configuration);

            services.AddMvc(mvcOptions =>
                {
                    mvcOptions.Filters.Add(
                        new AuthorizeFilter()
                    );
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddFluentValidation(configuration => 
                    configuration.RegisterValidatorsFromAssemblyContaining<SheepItModule>()
                );
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "sheepIt", Version = "v1" });
            });

            services.FixSwaggerRegistration();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // todo: do we really need this? we have SPA anyway
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
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            // todo: authorize all routes by default
            app.UseAuthentication();

            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
            );
            
            app.UseMvc();
            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }
    }
}