using System;

namespace ApiGatewayCustomAuthorizer
{
    public class ClaimsPrincipalException : BaseException
    {
        private static readonly string _name = nameof(ClaimsPrincipalException);
        public ClaimsPrincipalException() : base(_name) { }
        public ClaimsPrincipalException(string message) : base($"{_name}: {message}") { }
        public ClaimsPrincipalException(Exception inner) : base(_name, inner) { }
    }
}
