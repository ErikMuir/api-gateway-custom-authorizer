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
        private Mock<IPolicyBuilder> _policyBuilder;

        public AuthorizerFacadeTests()
        {
            _requestValidatorService = new Mock<IRequestValidationService>();
            _tokenConfigService = new Mock<ITokenConfigService>();
            _tokenValidationService = new Mock<ITokenValidationService>();
            _claimsPrincipalService = new Mock<IClaimsPrincipalService>();
            _policyBuilder = new Mock<IPolicyBuilder>();

            _testObject = new AuthorizerFacade(
                _requestValidatorService.Object,
                _tokenConfigService.Object,
                _tokenValidationService.Object,
                _claimsPrincipalService.Object,
                _policyBuilder.Object);
        }

        [Fact]
        public void Authorize_Throws_WhenRequestValidationServiceThrowsRequestValidationException()
        {
            _requestValidatorService
                .Setup(x => x.ValidateRequest(It.IsAny<Request>(), out It.Ref<ApiGatewayArn>.IsAny))
                .Throws<RequestValidationException>();

            var exception = Record.Exception(() => _testObject.Authorize(new Request()));

            Assert.NotNull(exception);
            Assert.IsType<RequestValidationException>(exception);
        }

        [Fact]
        public void Authorize_TokenValidationService_ValidateToken_Token()
        {
            var token = "some.auth.token";
            var request = new Request { AuthorizationToken = $"Bearer {token}" };

            _testObject.Authorize(request);

            _tokenValidationService.Verify(x => x.ValidateToken(token, It.IsAny<TokenValidationParameters>()), Times.Once);
        }

        [Fact]
        public void Authorize_TokenValidationService_ValidateToken_TokenValidationParameters()
        {
            var jwtConfig = new TokenValidationParameters();
            _tokenConfigService.Setup(x => x.GetJwtConfig()).Returns(jwtConfig);

            _testObject.Authorize(new Request());

            _tokenValidationService.Verify(x => x.ValidateToken(It.IsAny<string>(), jwtConfig), Times.Once);
        }

        [Fact]
        public void Authorize_ClaimsPrincipalService_GetPrincipalId()
        {
            var user = new ClaimsPrincipal();
            _tokenValidationService
                .Setup(x => x.ValidateToken(It.IsAny<string>(), It.IsAny<TokenValidationParameters>()))
                .Returns(user);

            _testObject.Authorize(new Request());

            _claimsPrincipalService.Verify(x => x.GetPrincipalId(user), Times.Once);
        }

        [Fact]
        public void Authorize_Calls_PolicyBuilder_AllowAllMethods_WhenTokenValidationSucceeds()
        {
            _testObject.Authorize(new Request());

            _policyBuilder.Verify(x => x.AllowAllMethods(), Times.Once);
            _policyBuilder.Verify(x => x.DenyAllMethods(), Times.Never);
        }

        [Fact]
        public void Authorize_Calls_PolicyBuilder_DenyAllMethods_WhenTokenConfigServiceThrowsJsonWebKeyClientException()
        {
            _tokenConfigService.Setup(x => x.GetJwtConfig()).Throws<JsonWebKeyClientException>();

            var actual = _testObject.Authorize(new Request());

            _policyBuilder.Verify(x => x.DenyAllMethods(), Times.Once);
            _policyBuilder.Verify(x => x.AllowAllMethods(), Times.Never);
        }

        [Fact]
        public void Authorize_Calls_PolicyBuilder_DenyAllMethods_WhenTokenConfigServiceThrowsJsonWebKeysServiceException()
        {
            _tokenConfigService.Setup(x => x.GetJwtConfig()).Throws<JsonWebKeyServiceException>();

            var actual = _testObject.Authorize(new Request());

            _policyBuilder.Verify(x => x.DenyAllMethods(), Times.Once);
            _policyBuilder.Verify(x => x.AllowAllMethods(), Times.Never);
        }

        [Fact]
        public void Authorize_Calls_PolicyBuilder_DenyAllMethods_WhenTokenValidationServiceThrowsTokenValidationException()
        {
            _tokenValidationService
                .Setup(x => x.ValidateToken(It.IsAny<string>(), It.IsAny<TokenValidationParameters>()))
                .Throws<TokenValidationException>();

            var actual = _testObject.Authorize(new Request());

            _policyBuilder.Verify(x => x.DenyAllMethods(), Times.Once);
            _policyBuilder.Verify(x => x.AllowAllMethods(), Times.Never);
        }

        [Fact]
        public void Authorize_Calls_PolicyBuilder_DenyAllMethods_WhenClaimsPrincipalServiceThrowsClaimsPrincipalException()
        {
            _claimsPrincipalService
                .Setup(x => x.GetPrincipalId(It.IsAny<ClaimsPrincipal>()))
                .Throws<ClaimsPrincipalException>();

            var actual = _testObject.Authorize(new Request());

            _policyBuilder.Verify(x => x.DenyAllMethods(), Times.Once);
            _policyBuilder.Verify(x => x.AllowAllMethods(), Times.Never);
        }

        [Fact]
        public void Authorize_Calls_PolicyBuilder_Build()
        {
            var apiGatewayArn = new ApiGatewayArn();
            var principalId = "fake-principal-id";
            _requestValidatorService.Setup(x => x.ValidateRequest(It.IsAny<Request>(), out apiGatewayArn));
            _claimsPrincipalService.Setup(x => x.GetPrincipalId(It.IsAny<ClaimsPrincipal>())).Returns(principalId);

            _testObject.Authorize(new Request());

            _policyBuilder.Verify(x => x.Build(apiGatewayArn, principalId), Times.Once);
        }

        [Fact]
        public void Authorize_Returns()
        {
            var response = new Response();
            _policyBuilder.Setup(x => x.Build(It.IsAny<ApiGatewayArn>(), It.IsAny<string>())).Returns(response);

            var actual = _testObject.Authorize(new Request());

            Assert.Equal(response, actual);
        }
    }
}
