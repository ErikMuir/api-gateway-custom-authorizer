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
        private readonly ITokenValidationService _tokenValidationService;

        public AuthorizerFacade() : this(new RequestValidationService(), new TokenConfigService(), new TokenValidationService()) { }

        public AuthorizerFacade(IRequestValidationService requestValidationService, ITokenConfigService tokenConfigService, ITokenValidationService tokenValidationService)
        {
            _requestValidationService = requestValidationService;
            _tokenConfigService = tokenConfigService;
            _tokenValidationService = tokenValidationService;
        }

        public bool Authorize(Request request, out ApiGatewayArn apiGatewayArn, out string principalId)
        {
            apiGatewayArn = null;
            principalId = null;

            try
            {
                _requestValidationService.ValidateRequest(request, out apiGatewayArn);

                var jwtConfig = _tokenConfigService.GetJwtConfig();

                var token = request.AuthorizationToken?.Replace("Bearer ", "");
                var claimsPrincipal = _tokenValidationService.ValidateToken(token, jwtConfig);

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
