using System;

namespace ApiGatewayCustomAuthorizer
{
    public class AuthPolicyBuilderException : BaseException
    {
        private static readonly string _name = nameof(AuthPolicyBuilderException);
        public AuthPolicyBuilderException() : base(_name) { }
        public AuthPolicyBuilderException(string message) : base($"{_name}: {message}") { }
        public AuthPolicyBuilderException(Exception inner) : base(_name, inner) { }
    }
}
