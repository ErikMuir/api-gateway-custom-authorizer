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
        private readonly IJsonWebKeyClient _jwkClient;

        public TokenConfigService() : this(EnvironmentWrapper.Instance, new JsonWebKeyService(), new JsonWebKeyClient()) { }

        public TokenConfigService(IEnvironmentWrapper env, IJsonWebKeyService jwkService, IJsonWebKeyClient jwkClient)
        {
            _env = env;
            _jwkService = jwkService;
            _jwkClient = jwkClient;
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
            // For best performace, store your jwks in an environment variable on this Lambda.
            // Just remember to update it anytime you cycle them with your auth provider.
            // It is highly recommended that you NOT rely on the HttpClient, as it will greatly increase latency.
            var jwks = GetJwksEnv() ?? GetJwksHttp();

            try
            {
                _logger.LogTrace("Retrieving signing keys from json", new { jwks });

                var signingKeys = _jwkService.GetSigningKeys(jwks);

                _logger.LogTrace("Successfuly retrieved signing keys from json", new { signingKeys });

                return signingKeys;
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not deserialize the signing keys", new { jwks });
                throw new JsonWebKeyServiceException(ex);
            }
        }

        private string GetJwksEnv()
        {
            return _env.Jwks.DefaultTo(null);
        }

        private string GetJwksHttp()
        {
            var jwksUri = _env.JwksUri;

            try
            {
                _logger.LogTrace("Sending jwks request", new { jwksUri });

                var jwks = _jwkClient.GetJwks(jwksUri);

                _logger.LogTrace("Received jwks response", new { jwks });

                Environment.SetEnvironmentVariable("JWKS", jwks);

                return jwks.DefaultTo(null);
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not successfully communicate with the auth service's jwks endpoint", new { jwksUri });
                throw new JsonWebKeyClientException(ex);
            }
        }
    }
}
