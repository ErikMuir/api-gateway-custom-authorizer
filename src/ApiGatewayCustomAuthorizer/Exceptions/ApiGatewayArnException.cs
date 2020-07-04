using System;

namespace ApiGatewayCustomAuthorizer
{
    public class ApiGatewayArnException : BaseException
    {
        private static readonly string _name = nameof(ApiGatewayArnException);
        public ApiGatewayArnException() : base(_name) { }
        public ApiGatewayArnException(string message) : base($"{_name}: {message}") { }
        public ApiGatewayArnException(Exception inner) : base(_name, inner) { }
    }
}
