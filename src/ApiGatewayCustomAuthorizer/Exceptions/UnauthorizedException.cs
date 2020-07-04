using System;

namespace ApiGatewayCustomAuthorizer
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException() : base("Unauthorized") { }
    }
}
