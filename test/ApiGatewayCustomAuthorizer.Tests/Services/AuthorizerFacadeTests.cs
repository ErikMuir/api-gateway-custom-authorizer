using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

namespace ApiGatewayCustomAuthorizer.Tests
{
    public class AuthorizerFacadeTests
    {
        private AuthorizerFacade _testObject;
        private Mock<IRequestValidationService> _requestValidatorService;
        private Mock<ITokenConfigService> _tokenConfigService;
        private Mock<ITokenValidationService> _tokenValidationService;
        private Mock<IClaimsPrincipalService> _claimsPrincipalService;

        public AuthorizerFacadeTests()
        {
            _requestValidatorService = new Mock<IRequestValidationService>();
            _tokenConfigService = new Mock<ITokenConfigService>();
            _tokenValidationService = new Mock<ITokenValidationService>();
            _claimsPrincipalService = new Mock<IClaimsPrincipalService>();

            _testObject = new AuthorizerFacade(_requestValidatorService.Object, _tokenConfigService.Object, _tokenValidationService.Object, _claimsPrincipalService.Object);
        }

        [Fact]
        public void Authorize_ReturnsTrue()
        {
            var actual = _testObject.Authorize(new Request(), out _, out _);

            Assert.True(actual);
        }

        [Fact]
        public void Authorize_Throws_WhenRequestValidationServiceThrowsRequestValidationException()
        {
            _requestValidatorService
                .Setup(x => x.ValidateRequest(It.IsAny<Request>(), out It.Ref<ApiGatewayArn>.IsAny))
                .Throws<RequestValidationException>();

            var exception = Record.Exception(() => _testObject.Authorize(new Request(), out _, out _));

            Assert.NotNull(exception);
            Assert.IsType<RequestValidationException>(exception);
        }

        [Fact]
        public void Authorize_ReturnsFalse_WhenTokenConfigServiceThrowsJsonWebKeyClientException()
        {
            _tokenConfigService.Setup(x => x.GetJwtConfig()).Throws<JsonWebKeyClientException>();

            var actual = _testObject.Authorize(new Request(), out _, out _);

            Assert.False(actual);
        }

        [Fact]
        public void Authorize_ReturnsFalse_WhenTokenConfigServiceThrowsJsonWebKeysServiceException()
        {
            _tokenConfigService.Setup(x => x.GetJwtConfig()).Throws<JsonWebKeyServiceException>();

            var actual = _testObject.Authorize(new Request(), out _, out _);

            Assert.False(actual);
        }

        [Fact]
        public void Authorize_ReturnsFalse_WhenTokenValidationServiceThrowsTokenValidationException()
        {
            _tokenValidationService
                .Setup(x => x.ValidateToken(It.IsAny<string>(), It.IsAny<TokenValidationParameters>()))
                .Throws<TokenValidationException>();

            var actual = _testObject.Authorize(new Request(), out _, out _);

            Assert.False(actual);
        }

        [Fact]
        public void Authorize_ReturnsFalse_WhenClaimsPrincipalServiceThrowsClaimsPrincipalException()
        {
            _claimsPrincipalService
                .Setup(x => x.GetPrincipalId(It.IsAny<ClaimsPrincipal>()))
                .Throws<ClaimsPrincipalException>();

            var actual = _testObject.Authorize(new Request(), out _, out _);

            Assert.False(actual);
        }
    }
}
