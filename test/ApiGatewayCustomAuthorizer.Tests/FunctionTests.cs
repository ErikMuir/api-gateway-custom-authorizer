using System;
using Amazon.Lambda.TestUtilities;
using Moq;
using Xunit;

namespace ApiGatewayCustomAuthorizer.Tests
{
    public class FunctionTests
    {
        private readonly IEnvironmentWrapper _env;
        private readonly Mock<IPolicyBuilder> _policyBuilder;
        private readonly Function _testObject;

        public FunctionTests()
        {
            _env = EnvironmentWrapper.Instance;
            _policyBuilder = new Mock<IPolicyBuilder>();
            _testObject = new Function(_env, _policyBuilder.Object);
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
        public void FunctionHandler_Calls_PolicyBuilder_DenyAllMethods()
        {
            var request = new Request { MethodArn = "arn:partition:service:region:awsAccountId:restApiId/stage/verb" };
            
            _testObject.FunctionHandler(request, new TestLambdaContext());

            _policyBuilder.Verify(x => x.DenyAllMethods(), Times.Once);
            _policyBuilder.Verify(x => x.AllowAllMethods(), Times.Never);
        }

        [Fact]
        public void FunctionHandler_Returns()
        {
            var request = new Request { MethodArn = "arn:partition:service:region:awsAccountId:restApiId/stage/verb" };
            var response = new Response();
            _policyBuilder.Setup(x => x.Build(It.IsAny<ApiGatewayArn>(), It.IsAny<string>())).Returns(response);

            var actual = _testObject.FunctionHandler(request, new TestLambdaContext());

            Assert.Equal(response, actual);
        }
    }
}
