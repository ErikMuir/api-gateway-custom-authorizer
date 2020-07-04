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

        public TokenConfigServiceTests()
        {
            _env = new Mock<IEnvironmentWrapper>();

            _testObject = new TokenConfigService(_env.Object);
        }

        [Fact]
        public void GetJwtConfig_Returns_TokenValidationParameters()
        {
            var issuer = "fake-issuer";
            var audience = "fake-audience";
            var expectedKeys = null as List<SecurityKey>;
            _env.Setup(e => e.Issuer).Returns(issuer);
            _env.Setup(e => e.Audience).Returns(audience);

            var actual = _testObject.GetJwtConfig();

            Assert.Equal(issuer, actual.ValidIssuer);
            Assert.Equal(audience, actual.ValidAudience);
            Assert.Equal(expectedKeys, actual.IssuerSigningKeys);
        }
    }
}
