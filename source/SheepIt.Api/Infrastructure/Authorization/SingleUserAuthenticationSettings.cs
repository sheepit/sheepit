using System;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace SheepIt.Api.Infrastructure.Authorization
{
    public class SingleUserAuthenticationSettings
    {
        public static SingleUserAuthenticationSettings FromConfiguration(IConfiguration configuration)
        {
            var configurationSectionName = "SingleUserAuthentication";
            
            var settings = configuration
                .GetSection(configurationSectionName)
                .Get<SettingsGroup>();
            
            if (settings.SecretKey == null)
            {
                throw new InvalidOperationException(
                    $"Couldn't find {settings.SecretKey} in {configurationSectionName} settings group."
                );
            }

            return new SingleUserAuthenticationSettings(
                secretKey: settings.SecretKey,
                singleUserPassword: settings.SingleUserPassword
            );
        }

        // ReSharper disable once ClassNeverInstantiated.Local - it's instantiated by configuration
        private class SettingsGroup
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local - it's used by configuration
            public string SecretKey { get; set; }
            
            // ReSharper disable once UnusedAutoPropertyAccessor.Local - it's used by configuration
            public string SingleUserPassword { get; set; }
        }

        public SymmetricSecurityKey SecretKey { get; }
        public string SingleUserPassword { get; }


        public SingleUserAuthenticationSettings(string secretKey, string singleUserPassword)
        {
            SecretKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(secretKey)
            );
            
            SingleUserPassword = singleUserPassword;
        }
    }
}