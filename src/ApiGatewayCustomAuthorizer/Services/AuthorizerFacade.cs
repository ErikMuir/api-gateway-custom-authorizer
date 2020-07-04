namespace ApiGatewayCustomAuthorizer
{
    public interface IAuthorizerFacade
    {
        bool Authorize(Request request, out ApiGatewayArn apiGatewayArn, out string principalId);
    }

    public class AuthorizerFacade : IAuthorizerFacade
    {
        private readonly Logger _logger = Logger.Create<AuthorizerFacade>();

        public bool Authorize(Request request, out ApiGatewayArn apiGatewayArn, out string principalId)
        {
            apiGatewayArn = null;
            principalId = null;

            try
            {
                // TODO : validate the request

                // TODO : set apiGatewayArn

                // TODO : get jwt config

                // TODO : validate token

                // TODO : set principalId

                // TODO : return true;
            }
            catch (BaseException ex)
            {
                _logger.LogWarning(ex.Message);
            }

            return false;
        }
    }
}
