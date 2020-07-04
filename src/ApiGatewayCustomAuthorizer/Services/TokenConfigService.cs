using Microsoft.IdentityModel.Tokens;

namespace ApiGatewayCustomAuthorizer
{
    public interface ITokenConfigService
    {
        TokenValidationParameters GetJwtConfig();
    }

    public class TokenConfigService : ITokenConfigService
    {
        private static readonly Logger _logger = Logger.Create<TokenConfigService>();
        private readonly IEnvironmentWrapper _env;

        public TokenConfigService() : this(EnvironmentWrapper.Instance) { }

        public TokenConfigService(IEnvironmentWrapper env)
        {
            _env = env;
        }

        public TokenValidationParameters GetJwtConfig()
        {
            _logger.LogTrace("Building jwt config");

            var jwtConfig = new TokenValidationParameters
            {
                ValidIssuer = _env.Issuer,
                ValidAudience = _env.Audience,
                IssuerSigningKeys = null, // TODO : get signing keys
            };

            _logger.LogTrace("Successfully built jwt config", new { jwtConfig });

            return jwtConfig;
        }
    }
}
