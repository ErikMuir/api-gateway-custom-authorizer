using System.Linq;
using System.Security.Claims;

namespace ApiGatewayCustomAuthorizer
{
    public interface IClaimsPrincipalService
    {
        string GetPrincipalId(ClaimsPrincipal user);
    }

    public class ClaimsPrincipalService : IClaimsPrincipalService
    {
        private static readonly Logger _logger = Logger.Create<ClaimsPrincipalService>();

        public string GetPrincipalId(ClaimsPrincipal user)
        {
            _logger.LogTrace("Parsing principalId from claims principal");

            if (user == null)
                throw new ClaimsPrincipalException("User is null");

            var nameIdentifier = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            if (nameIdentifier == null)
                throw new ClaimsPrincipalException("User does not have a NameIdentifier claim");

            var principalId = nameIdentifier.Value;

            _logger.LogTrace("Successfully parsed principalId", new { principalId });

            return principalId;
        }
    }
}
