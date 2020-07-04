using Xunit;

namespace ApiGatewayCustomAuthorizer.Tests
{
    public class RequestValidationServiceTests
    {
        private const string FakeType = "TOKEN";
        private const string FakeToken = "Bearer xyz";
        private const string FakeMethodArn = "arn:partition:service:region:awsAccountId:restApiId/stage/verb";

        private RequestValidationService _testObject;

        public RequestValidationServiceTests()
        {
            _testObject = new RequestValidationService();
        }

        [Fact]
        public void ValidateRequest_Throws_WhenRequestIsNull()
        {
            var request = null as Request;

            var exception = Record.Exception(() => _testObject.ValidateRequest(request, out _));

            Assert.NotNull(exception);
            Assert.IsType<RequestValidationException>(exception);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\t\n")]
        [InlineData("foobar")]
        public void ValidateRequest_Throws_WhenTypeIsInvalid(string type)
        {
            var request = new Request
            {
                Type = type,
                AuthorizationToken = FakeToken,
                MethodArn = FakeMethodArn,
            };

            var exception = Record.Exception(() => _testObject.ValidateRequest(request, out _));

            Assert.NotNull(exception);
            Assert.IsType<RequestValidationException>(exception);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\t\n")]
        [InlineData("invalid-auth-token")]
        [InlineData("Bearer")]
        [InlineData(" Bearer xyz")]
        [InlineData("Bearerxyz")]
        public void ValidateRequest_Throws_WhenAuthorizationTokenIsInvalid(string authToken)
        {
            var request = new Request
            {
                Type = FakeType,
                AuthorizationToken = authToken,
                MethodArn = FakeMethodArn,
            };

            var exception = Record.Exception(() => _testObject.ValidateRequest(request, out _));

            Assert.NotNull(exception);
            Assert.IsType<RequestValidationException>(exception);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\t\n")]
        [InlineData("invalid:method:arn")]
        public void ValidateRequest_Throws_WhenMethodArnIsInvalid(string methodArn)
        {
            var request = new Request
            {
                Type = FakeType,
                AuthorizationToken = FakeToken,
                MethodArn = methodArn,
            };

            var exception = Record.Exception(() => _testObject.ValidateRequest(request, out _));

            Assert.NotNull(exception);
            Assert.IsType<RequestValidationException>(exception);
        }

        [Fact]
        public void ValidateRequest_DoesNotThrow_WhenRequestIsValid()
        {
            var request = new Request
            {
                Type = FakeType,
                AuthorizationToken = FakeToken,
                MethodArn = FakeMethodArn,
            };

            var exception = Record.Exception(() => _testObject.ValidateRequest(request, out _));

            Assert.Null(exception);
        }

        [Fact]
        public void ValidateRequest_Sets_ApiGatewayArn()
        {
            var request = new Request
            {
                Type = FakeType,
                AuthorizationToken = FakeToken,
                MethodArn = FakeMethodArn,
            };
            var expected = ApiGatewayArn.Parse(FakeMethodArn);

            _testObject.ValidateRequest(request, out var actual);

            Assert.Equal(expected.ToString(), actual.ToString());
        }
    }
}
