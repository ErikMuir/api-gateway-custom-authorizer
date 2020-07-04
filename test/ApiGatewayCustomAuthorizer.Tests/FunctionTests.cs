using Amazon.Lambda.TestUtilities;
using Moq;
using Xunit;

namespace ApiGatewayCustomAuthorizer.Tests
{
    public class FunctionTests
    {
        private readonly IEnvironmentWrapper _env;
        private readonly Mock<IAuthorizerFacade> _authorizerFacade;
        private readonly Mock<IPolicyBuilder> _policyBuilder;
        private readonly Function _testObject;

        public FunctionTests()
        {
            _env = EnvironmentWrapper.Instance;
            _authorizerFacade = new Mock<IAuthorizerFacade>();
            _policyBuilder = new Mock<IPolicyBuilder>();
            _testObject = new Function(_env, _authorizerFacade.Object, _policyBuilder.Object);
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

            _authorizerFacade.Verify(x => x.Authorize(request, out It.Ref<ApiGatewayArn>.IsAny, out It.Ref<string>.IsAny), Times.Once);
        }

        [Fact]
        public void FunctionHandler_Calls_PolicyBuilder_AllowAllMethods_WhenAuthIsSuccessful()
        {
            _authorizerFacade
                .Setup(x => x.Authorize(It.IsAny<Request>(), out It.Ref<ApiGatewayArn>.IsAny, out It.Ref<string>.IsAny))
                .Returns(true);

            _testObject.FunctionHandler(new Request(), new TestLambdaContext());

            _policyBuilder.Verify(x => x.AllowAllMethods(), Times.Once);
            _policyBuilder.Verify(x => x.DenyAllMethods(), Times.Never);
        }

        [Fact]
        public void FunctionHandler_Calls_PolicyBuilder_DenyAllMethods_WhenAuthFails()
        {
            _authorizerFacade
                .Setup(x => x.Authorize(It.IsAny<Request>(), out It.Ref<ApiGatewayArn>.IsAny, out It.Ref<string>.IsAny))
                .Returns(false);

            _testObject.FunctionHandler(new Request(), new TestLambdaContext());

            _policyBuilder.Verify(x => x.DenyAllMethods(), Times.Once);
            _policyBuilder.Verify(x => x.AllowAllMethods(), Times.Never);
        }

        [Fact]
        public void FunctionHandler_Calls_PolicyBuilder_Build()
        {
            var apiGatewayArn = new ApiGatewayArn();
            var principalId = "fake-principal-id";
            _authorizerFacade.Setup(x => x.Authorize(It.IsAny<Request>(), out apiGatewayArn, out principalId));

            _testObject.FunctionHandler(new Request(), new TestLambdaContext());

            _policyBuilder.Verify(x => x.Build(apiGatewayArn, principalId), Times.Once);
        }

        [Fact]
        public void FunctionHandler_Returns()
        {
            var response = new Response();
            _policyBuilder.Setup(x => x.Build(It.IsAny<ApiGatewayArn>(), It.IsAny<string>())).Returns(response);

            var actual = _testObject.FunctionHandler(new Request(), new TestLambdaContext());

            Assert.Equal(response, actual);
        }
    }
}
