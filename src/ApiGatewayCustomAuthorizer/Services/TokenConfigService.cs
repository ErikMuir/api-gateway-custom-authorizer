using System;
using System.Collections.Generic;
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
        private readonly IJsonWebKeyService _jwkService;

        public TokenConfigService() : this(EnvironmentWrapper.Instance, new JsonWebKeyService()) { }

        public TokenConfigService(IEnvironmentWrapper env, IJsonWebKeyService jwkService)
        {
            _env = env;
            _jwkService = jwkService;
        }

        public TokenValidationParameters GetJwtConfig()
        {
            _logger.LogTrace("Building jwt config");

            var jwtConfig = new TokenValidationParameters
            {
                ValidIssuer = _env.Issuer,
                ValidAudience = _env.Audience,
                IssuerSigningKeys = GetSigningKeys(),
            };

            _logger.LogTrace("Successfully built jwt config", new { jwtConfig });

            return jwtConfig;
        }

        public ICollection<SecurityKey> GetSigningKeys()
        {
            // It is strongly recommended to store your jwks in an environment variable
            // to bypass the HttpClient dependency. You would just need to remember to
            // update the value whenever you cycle the keys.
            var jwks = _env.Jwks;

            try
            {
                _logger.LogTrace("Retrieving signing keys", new { jwks });

                var signingKeys = _jwkService.GetSigningKeys(jwks);

                _logger.LogTrace("Successfuly retrieved signing keys from json", new { signingKeys });

                return signingKeys;
            }
            catch (Exception ex)
            {
                throw new JsonWebKeyServiceException($"Could not deserialize the signing keys: {jwks}", ex);
            }
        }
    }
}
