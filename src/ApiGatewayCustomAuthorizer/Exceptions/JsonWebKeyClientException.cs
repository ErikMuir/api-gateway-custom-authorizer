using System;

namespace ApiGatewayCustomAuthorizer
{
    public class JsonWebKeyClientException : BaseException
    {
        public JsonWebKeyClientException() { }
        public JsonWebKeyClientException(string message) : base(message) { }
        public JsonWebKeyClientException(Exception inner) : base(nameof(JsonWebKeyClientException), inner) { }
        public JsonWebKeyClientException(string message, Exception inner) : base(message, inner) { }
    }
}
