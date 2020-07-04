using Moq;
using Xunit;

namespace ApiGatewayCustomAuthorizer.Tests
{
    public class AuthorizerFacadeTests
    {
        private AuthorizerFacade _testObject;
        private Mock<IRequestValidationService> _requestValidatorService;

        public AuthorizerFacadeTests()
        {
            _requestValidatorService = new Mock<IRequestValidationService>();

            _testObject = new AuthorizerFacade(_requestValidatorService.Object);
        }

        [Fact]
        public void Authorize_ReturnsFalse()
        {
            var actual = _testObject.Authorize(new Request(), out _, out _);

            Assert.False(actual);
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
    }
}
