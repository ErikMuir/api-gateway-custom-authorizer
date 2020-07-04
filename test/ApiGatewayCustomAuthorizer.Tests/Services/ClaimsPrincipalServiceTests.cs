using System.Collections.Generic;
using System.Security.Claims;
using Xunit;

namespace ApiGatewayCustomAuthorizer.Tests
{
    public class ClaimsPrincpalServiceTests
    {
        private ClaimsPrincipalService _testObject;

        public ClaimsPrincpalServiceTests()
        {
            _testObject = new ClaimsPrincipalService();
        }

        [Fact]
        public void GetPrincipalId_ReturnsPrincipalId()
        {
            var expected = "user-id";
            var identity = new ClaimsIdentity(new List<Claim> { new Claim(ClaimTypes.NameIdentifier, expected) });
            var user = new ClaimsPrincipal(identity);

            var actual = _testObject.GetPrincipalId(user);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetPrincipalId_Throws_WhenUserIsNull()
        {
            var user = null as ClaimsPrincipal;

            Assert.Throws<ClaimsPrincipalException>(() => _testObject.GetPrincipalId(user));
        }

        [Fact]
        public void GetPrincipalId_Throws_WhenUserHasNoClaims()
        {
            var user = new ClaimsPrincipal();

            Assert.Throws<ClaimsPrincipalException>(() => _testObject.GetPrincipalId(user));
        }

        [Fact]
        public void GetPrincipalId_Throws_WhenUserHasNoNameIdentityClaims()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new Claim(ClaimTypes.Email, "foobar@test.com") }));

            Assert.Throws<ClaimsPrincipalException>(() => _testObject.GetPrincipalId(user));
        }
    }
}
