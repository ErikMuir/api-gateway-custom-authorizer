using Xunit;

namespace ApiGatewayCustomAuthorizer.Tests
{
    public class AuthorizerFacadeTests
    {
        private AuthorizerFacade _testObject;

        public AuthorizerFacadeTests()
        {
            _testObject = new AuthorizerFacade();
        }

        [Fact]
        public void Authorize_ReturnsFalse()
        {
            var actual = _testObject.Authorize(new Request(), out _, out _);

            Assert.False(actual);
        }
    }
}
