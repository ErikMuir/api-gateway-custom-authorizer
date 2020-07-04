using System;
using System.Net.Http;

namespace ApiGatewayCustomAuthorizer
{
    public interface IJsonWebKeyClient
    {
        string GetJwks(string jwksUri);
    }

    public class JsonWebKeyClient : IJsonWebKeyClient
    {
        private static readonly Lazy<HttpClient> _httpClient = new Lazy<HttpClient>(() => new HttpClient());

        public string GetJwks(string jwksUri) => _httpClient.Value.GetStringAsync(jwksUri).Result;
    }
}
