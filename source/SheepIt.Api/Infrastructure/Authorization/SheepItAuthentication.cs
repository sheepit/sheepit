using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace SheepIt.Api.Infrastructure.Authorization
{
    public static class SheepItAuthentication
    {
        public static void AddSheepItAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var authorizationSettings = SingleUserAuthenticationSettings.FromConfiguration(configuration);

            services.AddSheepItAuthentication(authorizationSettings);
        }

        private static void AddSheepItAuthentication(this IServiceCollection services, SingleUserAuthenticationSettings authorizationSettings)
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    // todo: this should be dependent on whether it's a dev environment
                    options.RequireHttpsMetadata = false;
                    
                    options.SaveToken = true;
                    
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = authorizationSettings.SecretKey,
                        
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(1),
                        
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
        }
    }
}