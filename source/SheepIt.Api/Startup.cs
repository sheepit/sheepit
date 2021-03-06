using Autofac;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Infrastructure.Authorization;
using SheepIt.Api.Infrastructure.ErrorHandling;
using SheepIt.Api.Infrastructure.Logger;
using SheepIt.Api.Infrastructure.Swagger;

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

            services
                .AddControllers(mvcOptions =>
                {
                    mvcOptions.Filters.Add(
                        new AuthorizeFilter()
                    );
                })
                .AddNewtonsoftJson()
                .AddFluentValidation(configuration =>
                    configuration.RegisterValidatorsFromAssemblyContaining<SheepItModule>()
                );

            services.AddSwaggerDocumentation(_configuration);
            
            services.AddDbContext<SheepItDbContext>(options =>
                options.UseNpgsql(_configuration.GetConnectionString(SheepItDbContext.ConnectionStringName))
            );
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // todo: [rt] should this be here?
            using (var dbContext = app.ApplicationServices.GetService<SheepItDbContext>())
            {
                dbContext.Database.Migrate();
            }
            
            if (env.IsDevelopment())
            {
                // todo: do we really need this? we have SPA anyway
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors(options => options
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
            );

            app.UseHttpsRedirection();

            app.UseDefaultFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true
            });
            
            app.UseMiddleware<SerilogMiddleware>();
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            app.UseRouting();
            app.UseAuthorization();
            app.UseAuthentication();

            app.UseSwaggerDocumentation(_configuration);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        
        public void ConfigureContainer(ContainerBuilder builder) {
            builder.RegisterModule(new SheepItModule());
        }
    }
}