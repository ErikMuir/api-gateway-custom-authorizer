using System;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using Moq;
using Xunit;

namespace ApiGatewayCustomAuthorizer.Tests
{
    public class FunctionTests
    {
        private readonly Function _testObject;

        public FunctionTests()
        {
            _testObject = new Function();
        }

        [Fact]
        public void FunctionHandler_Throws()
        {
            var exception = Record.Exception(() => _testObject.FunctionHandler(It.IsAny<APIGatewayCustomAuthorizerRequest>(), new TestLambdaContext()));

            Assert.NotNull(exception);
            Assert.IsType<NotImplementedException>(exception);
        }
    }
}
