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

        [Fact]
        public void Authorize_TokenValidationService_ValidateToken_Token()
        {
            var token = "some.auth.token";
            var request = new Request { AuthorizationToken = $"Bearer {token}" };

            _testObject.Authorize(request, out _, out _);

            _tokenValidationService.Verify(x => x.ValidateToken(token, It.IsAny<TokenValidationParameters>()), Times.Once);
        }

        [Fact]
        public void Authorize_TokenValidationService_ValidateToken_TokenValidationParameters()
        {
            var jwtConfig = new TokenValidationParameters();
            _tokenConfigService.Setup(x => x.GetJwtConfig()).Returns(jwtConfig);

            _testObject.Authorize(new Request(), out _, out _);

            _tokenValidationService.Verify(x => x.ValidateToken(It.IsAny<string>(), jwtConfig), Times.Once);
        }

        [Fact]
        public void Authorize_ClaimsPrincipalService_GetPrincipalId()
        {
            var user = new ClaimsPrincipal();
            _tokenValidationService
                .Setup(x => x.ValidateToken(It.IsAny<string>(), It.IsAny<TokenValidationParameters>()))
                .Returns(user);

            _testObject.Authorize(new Request(), out _, out _);

            _claimsPrincipalService.Verify(x => x.GetPrincipalId(user), Times.Once);
        }

        [Fact]
        public void Authorize_Sets_ApiGatewayArn()
        {
            var apiGatewayArn = new ApiGatewayArn();
            _requestValidatorService.Setup(x => x.ValidateRequest(It.IsAny<Request>(), out apiGatewayArn));

            _testObject.Authorize(new Request(), out var actual, out _);

            Assert.Equal(apiGatewayArn, actual);
        }

        [Fact]
        public void Authorize_Sets_PrincipalId()
        {
            var principalId = "fake-user-id";
            _claimsPrincipalService
                .Setup(x => x.GetPrincipalId(It.IsAny<ClaimsPrincipal>()))
                .Returns(principalId);

            _testObject.Authorize(new Request(), out _, out var actual);

            Assert.Equal(principalId, actual);
        }
    }
}
