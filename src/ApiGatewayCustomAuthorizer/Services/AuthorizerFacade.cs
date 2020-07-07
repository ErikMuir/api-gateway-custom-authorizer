using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace ApiGatewayCustomAuthorizer
{
    public interface IAuthorizerFacade
    {
        Response Authorize(Request request);
    }

    public class AuthorizerFacade : IAuthorizerFacade
    {
        private readonly Logger _logger = Logger.Create<AuthorizerFacade>();
        private readonly IRequestValidationService _requestValidationService;
        private readonly ITokenConfigService _tokenConfigService;
        private readonly ITokenValidationService _tokenValidationService;
        private readonly IClaimsPrincipalService _claimsPrincipalService;
        private readonly IPolicyBuilder _policyBuilder;

        public AuthorizerFacade() : this(
            new RequestValidationService(),
            new TokenConfigService(),
            new TokenValidationService(),
            new ClaimsPrincipalService(),
            new PolicyBuilder()) { }

        public AuthorizerFacade(
            IRequestValidationService requestValidationService,
            ITokenConfigService tokenConfigService,
            ITokenValidationService tokenValidationService,
            IClaimsPrincipalService claimsPrincipalService,
            IPolicyBuilder policyBuilder)
        {
            _requestValidationService = requestValidationService;
            _tokenConfigService = tokenConfigService;
            _tokenValidationService = tokenValidationService;
            _claimsPrincipalService = claimsPrincipalService;
            _policyBuilder = policyBuilder;
        }

        public Response Authorize(Request request)
        {
            ApiGatewayArn apiGatewayArn = null;
            string principalId = null;

            try
            {
                _logger.LogDebug("Authorizing");

                _requestValidationService.ValidateRequest(request, out apiGatewayArn);

                TokenValidationParameters jwtConfig = _tokenConfigService.GetJwtConfig();

                string token = request.AuthorizationToken?.Replace("Bearer ", "");
                ClaimsPrincipal user = _tokenValidationService.ValidateToken(token, jwtConfig);

                principalId = _claimsPrincipalService.GetPrincipalId(user);

                // this uses the all-or-nothing auth strategy by default:
                _policyBuilder.AllowAllMethods();

                // however, you could allow/deny specific methods/resources based off the user's claims, for example:
                // Claim role = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);
                // HttpVerb verb = role?.Value == "admin" ? HttpVerb.All : HttpVerb.Get;
                // _policyBuilder.AllowMethod(verb, "config");

                _logger.LogDebug("Authorization succeeded", new { principalId });
            }
            catch (BaseException ex)
            {
                _logger.LogDebug(ex.Message, new { ex.InnerException });
                _logger.LogDebug("Authorization failed", new { });

                // you may choose to throw an unauthorized exception here instead of returning a "deny all" policy
                // throw new UnauthorizedException();
                
                _policyBuilder.DenyAllMethods();
            }

            Response response = _policyBuilder.Build(apiGatewayArn, principalId);

            // you can add key-value pairs that can be accessed in API Gateway via $context.authorizer.<key>
            // response.Context.Add("key", "value");

            return response;
        }
    }
}
