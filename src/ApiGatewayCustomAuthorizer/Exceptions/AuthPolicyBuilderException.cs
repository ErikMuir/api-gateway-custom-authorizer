using System;

namespace ApiGatewayCustomAuthorizer
{
    public class AuthPolicyBuilderException : BaseException
    {
        public AuthPolicyBuilderException() : base(nameof(AuthPolicyBuilderException)) { }
        public AuthPolicyBuilderException(string message) : base($"{nameof(AuthPolicyBuilderException)}: {message}") { }
        public AuthPolicyBuilderException(Exception inner) : base(nameof(AuthPolicyBuilderException), inner) { }
        public AuthPolicyBuilderException(string message, Exception inner) : base($"{nameof(AuthPolicyBuilderException)}: {message}", inner) { }
    }
}
