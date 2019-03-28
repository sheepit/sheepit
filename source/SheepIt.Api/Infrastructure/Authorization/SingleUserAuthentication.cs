using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace SheepIt.Api.Infrastructure.Authorization
{
    // this single-user authorization is a temporary solution for MVP version of the app
    
    public class SingleUserAuthentication
    {
        private readonly SingleUserAuthenticationSettings _settings;

        public SingleUserAuthentication(SingleUserAuthenticationSettings settings)
        {
            _settings = settings;
        }

        public void TryAuthenticate(string singleUserPassword, out string jwtTokenOrNull)
        {
            jwtTokenOrNull = singleUserPassword == _settings.SingleUserPassword
                ? CreateJwtToken()
                : null;
        }

        private string CreateJwtToken()
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var securityToken = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, "SheepIt user")
                }),
                
                // todo: shorter tokens + refresh token in http-only cookie would obviously be safer
                Expires = DateTime.UtcNow.AddDays(1),
                
                SigningCredentials = new SigningCredentials(
                    key: _settings.SecretKey,
                    algorithm: SecurityAlgorithms.HmacSha256Signature
                )
            });

            return tokenHandler.WriteToken(securityToken);
        }
    }
}