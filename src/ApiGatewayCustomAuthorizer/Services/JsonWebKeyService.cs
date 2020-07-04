using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;

namespace ApiGatewayCustomAuthorizer
{
    public interface IJsonWebKeyService
    {
        ICollection<SecurityKey> GetSigningKeys(string jwks);
    }

    public class JsonWebKeyService : IJsonWebKeyService
    {
        public ICollection<SecurityKey> GetSigningKeys(string jwks) => new JsonWebKeySet(jwks).GetSigningKeys();
    }
}
