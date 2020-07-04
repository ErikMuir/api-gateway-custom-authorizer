using System;

namespace ApiGatewayCustomAuthorizer
{
    public class TokenValidationException : BaseException
    {
        public TokenValidationException() { }
        public TokenValidationException(string message) : base(message) { }
        public TokenValidationException(Exception inner) : base(nameof(TokenValidationException), inner) { }
        public TokenValidationException(string message, Exception inner) : base(message, inner) { }
    }
}
