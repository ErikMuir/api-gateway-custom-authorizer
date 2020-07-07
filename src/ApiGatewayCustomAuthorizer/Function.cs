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

        public Function() : this(EnvironmentWrapper.Instance, new AuthorizerFacade())
        { }

        public Function(IEnvironmentWrapper env, IAuthorizerFacade authorizerFacade)
        {
            _env = env;
            _authorizerFacade = authorizerFacade;
        }

        public Response FunctionHandler(Request request, ILambdaContext context)
        {
            _env.Request = request;
            _env.Context = context;

            try
            {
                var response = _authorizerFacade.Authorize(request);

                _logger.LogInformation("Authorizer Response", new { response });

                return response;
            }
            catch (Exception ex)
            {
                if (ex is UnauthorizedException)
                    throw;

                _logger.LogError(ex, "Unknown error occurred");
                throw new UnauthorizedException();
            }
        }
    }
}
