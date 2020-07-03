using System;

namespace ApiGatewayCustomAuthorizer
{
    public class ApiGatewayArnException : BaseException
    {
        public ApiGatewayArnException() : base(nameof(ApiGatewayArnException)) { }
        public ApiGatewayArnException(string message) : base($"{nameof(ApiGatewayArnException)}: {message}") { }
        public ApiGatewayArnException(Exception inner) : base(nameof(ApiGatewayArnException), inner) { }
        public ApiGatewayArnException(string message, Exception inner) : base($"{nameof(ApiGatewayArnException)}: {message}", inner) { }
    }
}
