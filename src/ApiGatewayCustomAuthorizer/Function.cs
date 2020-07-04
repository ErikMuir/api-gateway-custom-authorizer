using System;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ApiGatewayCustomAuthorizer
{
    public class Function
    {
        private static readonly Logger _logger = Logger.Create<Function>();
        private readonly IEnvironmentWrapper _env;
        private readonly IAuthorizerFacade _authorizerFacade;
        private readonly IPolicyBuilder _policyBuilder;

        public Function() : this(EnvironmentWrapper.Instance, new AuthorizerFacade(), new PolicyBuilder())
        { }

        public Function(IEnvironmentWrapper env, IAuthorizerFacade authorizerFacade, IPolicyBuilder policyBuilder)
        {
            _env = env;
            _authorizerFacade = authorizerFacade;
            _policyBuilder = policyBuilder;
        }

        public Response FunctionHandler(Request request, ILambdaContext context)
        {
            _env.Request = request;
            _env.Context = context;

            var isAuthorized = _authorizerFacade.Authorize(request, out ApiGatewayArn apiGatewayArn, out string principalId);

            if (isAuthorized)
            {
                // you could implement claims-based authorization for each resource/method if desired
                // but this implementation is more of an all-or-nothing approach
                _policyBuilder.AllowAllMethods();
            }
            else
            {
                // you could throw an unauthorized exception here if desired
                // but this implementation returns a policy that denies all resources and methods
                _policyBuilder.DenyAllMethods();
            }

            var response = _policyBuilder.Build(apiGatewayArn, principalId);

            // you can add key-value pairs that can be accessed in API Gateway via $context.authorizer.<key>
            // response.Context.Add("key", "value");

            _logger.LogInformation("Authorizer Response", new { response });

            return response;
        }
    }
}
