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
        private readonly ITokenConfigService _tokenConfigService;

        public AuthorizerFacade() : this(new RequestValidationService(), new TokenConfigService()) { }

        public AuthorizerFacade(IRequestValidationService requestValidationService, ITokenConfigService tokenConfigService)
        {
            _requestValidationService = requestValidationService;
            _tokenConfigService = tokenConfigService;
        }

        public bool Authorize(Request request, out ApiGatewayArn apiGatewayArn, out string principalId)
        {
            apiGatewayArn = null;
            principalId = null;

            try
            {
                _requestValidationService.ValidateRequest(request, out apiGatewayArn);

                var jwtConfig = _tokenConfigService.GetJwtConfig();

                // TODO : validate token

                // TODO : set principalId

                return true;
            }
            catch (BaseException ex)
            {
                _logger.LogWarning(ex.Message);
            }

            return false;
        }
    }
}
