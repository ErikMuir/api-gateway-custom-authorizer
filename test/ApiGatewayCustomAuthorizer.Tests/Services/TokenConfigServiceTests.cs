using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

namespace ApiGatewayCustomAuthorizer.Tests
{
    public class TokenConfigServiceTests
    {
        private TokenConfigService _testObject;
        private Mock<IEnvironmentWrapper> _env;
        private Mock<IJsonWebKeyService> _jwkService;

        public TokenConfigServiceTests()
        {
            _env = new Mock<IEnvironmentWrapper>();
            _jwkService = new Mock<IJsonWebKeyService>();

            _testObject = new TokenConfigService(_env.Object, _jwkService.Object);
        }

        [Fact]
        public void GetJwtConfig_Returns_TokenValidationParameters()
        {
            var issuer = "fake-issuer";
            var audience = "fake-audience";
            var expectedKeys = new List<SecurityKey>();
            _env.Setup(e => e.Issuer).Returns(issuer);
            _env.Setup(e => e.Audience).Returns(audience);
            _jwkService.Setup(x => x.GetSigningKeys(It.IsAny<string>())).Returns(expectedKeys);

            var actual = _testObject.GetJwtConfig();

            Assert.Equal(issuer, actual.ValidIssuer);
            Assert.Equal(audience, actual.ValidAudience);
            Assert.Equal(expectedKeys, actual.IssuerSigningKeys);
        }

        [Fact]
        public void GetJwtConfig_JsonWebKeyService_GetSigningKeys()
        {
            var jwks = "{\"keys\": []}";
            _env.Setup(x => x.Jwks).Returns(jwks);

            _testObject.GetJwtConfig();

            _jwkService.Verify(x => x.GetSigningKeys(jwks), Times.Once);
        }
    }
}
