using System;

namespace ApiGatewayCustomAuthorizer
{
    public class JsonWebKeyServiceException : BaseException
    {
        private static readonly string _name = nameof(JsonWebKeyServiceException);
        public JsonWebKeyServiceException() : base(_name) { }
        public JsonWebKeyServiceException(string message) : base($"{_name}: {message}") { }
        public JsonWebKeyServiceException(Exception inner) : base(_name, inner) { }
    }
}
