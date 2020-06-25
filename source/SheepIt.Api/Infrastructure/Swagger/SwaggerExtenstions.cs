using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace SheepIt.Api.Infrastructure.Swagger
{
    internal static class SwaggerExtensions
    {
        internal static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services, IConfiguration configuration)
        {
            var enableSwagger = configuration.GetValue<bool>("Swagger:Enable");
            
            if (enableSwagger)
            {
                services
                    .AddSwaggerGen(c =>
                    {
                        c.EnableAnnotations();
                        c.SwaggerDoc("1.0.0", new OpenApiInfo
                        {
                            Version = "1.0.0",
                            Title = "SheepIT",
                            Description = "SheepIT API"
                        });
                        c.CustomSchemaIds(x => x.FullName);
                        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                        {
                            In = ParameterLocation.Header,
                            Description = "Paste JWT with Bearer into field",
                            Name = "Authorization",
                            Type = SecuritySchemeType.ApiKey
                        });
                        c.AddSecurityRequirement(new OpenApiSecurityRequirement
                        {
                            { 
                                new OpenApiSecurityScheme 
                                { 
                                    Reference = new OpenApiReference 
                                    { 
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "Bearer" 
                                    } 
                                },
                                new string[] { } 
                            } 
                        });
                    });
            }

            return services;
        }

        internal static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app, IConfiguration configuration)
        {
            var enableSwagger = configuration.GetValue<bool>("Swagger:Enable");
            
            if (enableSwagger)
            {
                app
                    .UseSwagger()
                    .UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/1.0.0/swagger.json", "SheepIT API");
                    });
            }

            return app;
        }
    }
}