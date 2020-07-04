using System;

namespace ApiGatewayCustomAuthorizer
{
    public class JsonWebKeyClientException : BaseException
    {
        private static readonly string _name = nameof(JsonWebKeyClientException);
        public JsonWebKeyClientException() : base(_name) { }
        public JsonWebKeyClientException(string message) : base($"{_name}: {message}") { }
        public JsonWebKeyClientException(Exception inner) : base(_name, inner) { }
    }
}
