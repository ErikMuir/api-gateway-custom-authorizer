using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

namespace ApiGatewayCustomAuthorizer.Tests
{
    public class TokenValidationServiceTests
    {
        private readonly TokenValidationService _testObject;
        private readonly Mock<ISecurityTokenValidator> _jwtValidator;

        public TokenValidationServiceTests()
        {
            _jwtValidator = new Mock<ISecurityTokenValidator>();

            _testObject = new TokenValidationService(_jwtValidator.Object);
        }

        [Fact]
        public void ValidateToken_CallsJwtValidator_WithCorrectArguments()
        {
            var token = "token";
            var config = new TokenValidationParameters();

            _testObject.ValidateToken(token, config);

            _jwtValidator.Verify(x => x.ValidateToken(token, config, out It.Ref<SecurityToken>.IsAny), Times.Once);
        }

        [Fact]
        public void ValidateToken_ReturnsClaimsPrincipal()
        {
            var claimsPrincipal = new ClaimsPrincipal();
            _jwtValidator
                .Setup(x => x.ValidateToken(It.IsAny<string>(), It.IsAny<TokenValidationParameters>(), out It.Ref<SecurityToken>.IsAny))
                .Returns(claimsPrincipal);

            var actual = _testObject.ValidateToken("token", new TokenValidationParameters());

            Assert.Equal(claimsPrincipal, actual);
        }
    }
}
