using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Infrastructure.Authorization;

namespace SheepIt.Api.UseCases.Authentication
{
    [Route("frontendApi")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly SingleUserAuthentication _singleUserAuthentication;

        public AuthenticationController(SingleUserAuthentication singleUserAuthentication)
        {
            _singleUserAuthentication = singleUserAuthentication;
        }

        [Route("authenticate")]
        [AllowAnonymous]
        public AuthenticationResponse Authenticate(AuthenticateRequest request)
        {
            _singleUserAuthentication.TryAuthenticate(request.Password, out var jwtTokenOrNull);
            
            if (jwtTokenOrNull != null)
            {
                return new AuthenticationResponse
                {
                    Authenticated = true,
                    JwtToken = jwtTokenOrNull
                };
            }
            
            return new AuthenticationResponse
            {
                Authenticated = false,
                JwtToken = null
            };
        }
    }

    public class AuthenticateRequest
    {
        public string Password { get; set; }
    }

    public class AuthenticationResponse
    {
        public bool Authenticated { get; set; }
        public string JwtToken { get; set; }
    }
}