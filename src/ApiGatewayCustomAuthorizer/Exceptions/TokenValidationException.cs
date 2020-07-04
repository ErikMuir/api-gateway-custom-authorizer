using System;

namespace ApiGatewayCustomAuthorizer
{
    public class TokenValidationException : BaseException
    {
        private static readonly string _name = nameof(TokenValidationException);
        public TokenValidationException() : base(_name) { }
        public TokenValidationException(string message) : base($"{_name}: {message}") { }
        public TokenValidationException(Exception inner) : base(_name, inner) { }
    }
}
