namespace ApiGatewayCustomAuthorizer
{
    public class RequestValidationException : UnauthorizedException
    {
        public RequestValidationException() : base() { }
    }
}
