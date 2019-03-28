using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Infrastructure.Authorization;

namespace SheepIt.Api.Tests.Infrastructure.Authorization
{
    public class SingleUserAuthenticationTests
    {
        private SingleUserAuthentication _singleUserAuthentication;

        [SetUp]
        public void set_up()
        {
            _singleUserAuthentication = new SingleUserAuthentication(
                new SingleUserAuthenticationSettings(
                    secretKey: "53c28115-533d-4a93-b97d-b232a5d02ebe",
                    singleUserPassword: "test-password"
                )
            );
        }
        
        [Test]
        public void returns_jwt_token_when_password_is_correct()
        {
            _singleUserAuthentication.TryAuthenticate("test-password", out var jwtTokenOrNull);

            jwtTokenOrNull.Should().NotBeNull();
        }

        [Test]
        public void does_not_return_jwt_token_when_password_is_incorrect()
        {
            _singleUserAuthentication.TryAuthenticate("invalid-password", out var jwtTokenOrNull);

            jwtTokenOrNull.Should().BeNull();
        }
    }
}