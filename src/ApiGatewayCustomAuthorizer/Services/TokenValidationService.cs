using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace ApiGatewayCustomAuthorizer
{
    public interface ITokenValidationService
    {
        ClaimsPrincipal ValidateToken(string token, TokenValidationParameters jwtConfig);
    }

    public class TokenValidationService : ITokenValidationService
    {
        private static readonly Logger _logger = Logger.Create<TokenValidationService>();
        private readonly ISecurityTokenValidator _jwtValidator;

        public TokenValidationService() : this(new JwtSecurityTokenHandler()) { }

        public TokenValidationService(ISecurityTokenValidator jwtValidator)
        {
            _jwtValidator = jwtValidator;
        }

        public ClaimsPrincipal ValidateToken(string token, TokenValidationParameters jwtConfig)
        {
            try
            {
                _logger.LogTrace("Validating token with config", new { jwtConfig });

                var claimsPrincipal = _jwtValidator.ValidateToken(token, jwtConfig, out _);

                _logger.LogTrace("Successfully validated token");

                return claimsPrincipal;
            }
            catch (Exception ex)
            {
                throw new TokenValidationException($"{ex.Message}", ex);
            }
        }
    }
}
