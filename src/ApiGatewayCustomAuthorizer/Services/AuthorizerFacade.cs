namespace ApiGatewayCustomAuthorizer
{
    public interface IAuthorizerFacade
    {
        bool Authorize(Request request, out ApiGatewayArn apiGatewayArn, out string principalId);
    }

    public class AuthorizerFacade : IAuthorizerFacade
    {
        private readonly Logger _logger = Logger.Create<AuthorizerFacade>();
        private readonly IRequestValidationService _requestValidationService;

        public AuthorizerFacade() : this(new RequestValidationService()) { }

        public AuthorizerFacade(IRequestValidationService requestValidationService)
        {
            _requestValidationService = requestValidationService;
        }

        public bool Authorize(Request request, out ApiGatewayArn apiGatewayArn, out string principalId)
        {
            apiGatewayArn = null;
            principalId = null;

            try
            {
                _requestValidationService.ValidateRequest(request, out apiGatewayArn);

                // TODO : get jwt config

                // TODO : validate token

                // TODO : set principalId

                // TODO : return true;
            }
            catch (BaseException ex)
            {
                _logger.LogWarning(ex.Message);
            }

            return false;
        }
    }
}
