using System;
using Amazon.Lambda.TestUtilities;
using Moq;
using Xunit;

namespace ApiGatewayCustomAuthorizer.Tests
{
    public class FunctionTests
    {
        private readonly IEnvironmentWrapper _env;
        private readonly Mock<IAuthorizerFacade> _authorizerFacade;
        private readonly Function _testObject;

        public FunctionTests()
        {
            _env = EnvironmentWrapper.Instance;
            _authorizerFacade = new Mock<IAuthorizerFacade>();
            _testObject = new Function(_env, _authorizerFacade.Object);
        }

        [Fact]
        public void FunctionHandler_EnvironmentWrapper_SetsRequest()
        {
            var request = new Request();

            Record.Exception(() => _testObject.FunctionHandler(request, new TestLambdaContext()));

            Assert.Same(request, _env.Request);
        }

        [Fact]
        public void FunctionHandler_EnvironmentWrapper_SetsContext()
        {
            var context = new TestLambdaContext();

            Record.Exception(() => _testObject.FunctionHandler(new Request(), context));

            Assert.Same(context, _env.Context);
        }

        [Fact]
        public void FunctionHandler_Calls_AuthorizerFacade_Authorize()
        {
            var request = new Request();

            Record.Exception(() => _testObject.FunctionHandler(request, new TestLambdaContext()));

            _authorizerFacade.Verify(x => x.Authorize(request), Times.Once);
        }

        [Fact]
        public void FunctionHandler_Catches_Exception_Throws_UnauthorizedException()
        {
            _authorizerFacade.Setup(x => x.Authorize(It.IsAny<Request>())).Throws<Exception>();

            var exception = Record.Exception(() => _testObject.FunctionHandler(new Request(), new TestLambdaContext()));

            Assert.NotNull(exception);
            Assert.IsType<UnauthorizedException>(exception);
        }

        [Fact]
        public void FunctionHandler_Catches_UnauthorizedException_Rethrows()
        {
            var unauthorizedException = new UnauthorizedException();
            _authorizerFacade.Setup(x => x.Authorize(It.IsAny<Request>())).Throws(unauthorizedException);

            var actualException = Record.Exception(() => _testObject.FunctionHandler(new Request(), new TestLambdaContext()));

            Assert.Same(unauthorizedException, actualException);
        }
    }
}
