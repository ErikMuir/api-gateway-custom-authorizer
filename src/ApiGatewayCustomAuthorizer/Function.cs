using System;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ApiGatewayCustomAuthorizer
{
    public class Function
    {
        public void FunctionHandler(string request, ILambdaContext context)
        {
            throw new NotImplementedException();
        }
    }
}
