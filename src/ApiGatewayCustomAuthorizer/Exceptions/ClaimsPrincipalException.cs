using System;

namespace ApiGatewayCustomAuthorizer
{
    public class ClaimsPrincipalException : BaseException
    {
        public ClaimsPrincipalException() { }
        public ClaimsPrincipalException(string message) : base(message) { }
        public ClaimsPrincipalException(Exception inner) : base(nameof(ClaimsPrincipalException), inner) { }
        public ClaimsPrincipalException(string message, Exception inner) : base(message, inner) { }
    }
}
