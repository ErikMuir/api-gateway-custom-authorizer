using System;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

namespace ApiGatewayCustomAuthorizer
{
    public interface IEnvironmentWrapper
    {
        string Issuer { get; }
        string Audience { get; }
        string Jwks { get; }
        string JwksUri { get; }
        string LogLevel { get; }
        string Version { get; }
        APIGatewayCustomAuthorizerRequest Request { get; set; }
        ILambdaContext Context { get; set; }
    }

    public sealed class EnvironmentWrapper : IEnvironmentWrapper
    {
        private EnvironmentWrapper() { }

        public static readonly EnvironmentWrapper Instance = new EnvironmentWrapper();

        public string Issuer => Environment.GetEnvironmentVariable("ISSUER");

        public string Audience => Environment.GetEnvironmentVariable("AUDIENCE");

        public string Jwks => Environment.GetEnvironmentVariable("JWKS");

        public string JwksUri => Environment.GetEnvironmentVariable("JWKS_URI");

        public string LogLevel => Environment.GetEnvironmentVariable("LOG_LEVEL");

        public string Version => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public APIGatewayCustomAuthorizerRequest Request { get; set; }

        public ILambdaContext Context { get; set; }
    }
}
