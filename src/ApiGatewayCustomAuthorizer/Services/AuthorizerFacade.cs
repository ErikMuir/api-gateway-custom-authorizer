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
        private readonly IClaimsPrincipalService _claimsPrincipalService;

        public AuthorizerFacade() : this(new RequestValidationService(), new TokenConfigService(), new TokenValidationService(), new ClaimsPrincipalService()) { }

        public AuthorizerFacade(IRequestValidationService requestValidationService, ITokenConfigService tokenConfigService, ITokenValidationService tokenValidationService, IClaimsPrincipalService claimsPrincipalService)
        {
            _requestValidationService = requestValidationService;
            _tokenConfigService = tokenConfigService;
            _tokenValidationService = tokenValidationService;
            _claimsPrincipalService = claimsPrincipalService;
        }

        public bool Authorize(Request request, out ApiGatewayArn apiGatewayArn, out string principalId)
        {
            apiGatewayArn = null;
            principalId = null;

            var isAuthorized = false;

            try
            {
                _logger.LogDebug("Authorizing");

                _requestValidationService.ValidateRequest(request, out apiGatewayArn);

                var jwtConfig = _tokenConfigService.GetJwtConfig();

                var token = request.AuthorizationToken?.Replace("Bearer ", "");
                var user = _tokenValidationService.ValidateToken(token, jwtConfig);

                principalId = _claimsPrincipalService.GetPrincipalId(user);

                isAuthorized = true;
            }
            catch (BaseException ex)
            {
                _logger.LogDebug(ex.Message, new { ex.InnerException });
            }

            _logger.LogDebug($"Authorization {(isAuthorized ? "succeeded" : "failed")}", new { principalId });

            return isAuthorized;
        }
    }
}
