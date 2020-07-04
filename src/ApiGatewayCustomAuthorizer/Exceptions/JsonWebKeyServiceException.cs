using System;

namespace ApiGatewayCustomAuthorizer
{
    public class JsonWebKeyServiceException : BaseException
    {
        public JsonWebKeyServiceException() { }
        public JsonWebKeyServiceException(string message) : base(message) { }
        public JsonWebKeyServiceException(Exception inner) : base(nameof(JsonWebKeyServiceException), inner) { }
        public JsonWebKeyServiceException(string message, Exception inner) : base(message, inner) { }
    }
}
