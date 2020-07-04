using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ApiGatewayCustomAuthorizer
{
    public interface IRequestValidationService
    {
        void ValidateRequest(Request request, out ApiGatewayArn apiGatewayArn);
    }

    public class RequestValidationService : IRequestValidationService
    {
        private static readonly Logger _logger = Logger.Create<RequestValidationService>();

        public void ValidateRequest(Request request, out ApiGatewayArn apiGatewayArn)
        {
            _logger.LogTrace("Validating request");

            apiGatewayArn = null;
            var validationMessages = new List<string>();

            if (request == null)
            {
                validationMessages.Add("Request is required.");
            }
            else
            {
                if (request.Type == null)
                {
                    validationMessages.Add("Expected 'type' to have been provided.");
                }
                else if (request.Type.ToUpperInvariant() != "TOKEN")
                {
                    validationMessages.Add("Expected 'type' to have value 'TOKEN'.");
                }

                if (string.IsNullOrWhiteSpace(request.AuthorizationToken))
                {
                    validationMessages.Add("Expected 'authorizationToken' to have been provided.");
                }
                else if (!new Regex("^Bearer .*$").IsMatch(request.AuthorizationToken))
                {
                    validationMessages.Add("Expected 'authorizationToken' to match '^Bearer .*$'.");
                }

                if (string.IsNullOrWhiteSpace(request.MethodArn))
                {
                    validationMessages.Add("Expected 'methodArn' to have been provided.");
                }
                else if (!ApiGatewayArn.TryParse(request.MethodArn, out apiGatewayArn))
                {
                    validationMessages.Add($"Could not parse 'methodArn': '{request.MethodArn}'.");
                }
            }

            if (validationMessages.Count > 0)
            {
                _logger.LogError("Request validation failed", new { validationMessages });
                throw new RequestValidationException();
            }

            _logger.LogTrace("Successfully validated request");
        }
    }
}
