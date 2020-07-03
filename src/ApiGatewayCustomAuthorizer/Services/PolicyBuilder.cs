using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Amazon.Lambda.APIGatewayEvents;
using static Amazon.Lambda.APIGatewayEvents.APIGatewayCustomAuthorizerPolicy;

namespace ApiGatewayCustomAuthorizer
{
    public interface IPolicyBuilder
    {
        void AllowAllMethods();
        void AllowMethod(HttpVerb verb, string resource);
        void DenyAllMethods();
        void DenyMethod(HttpVerb verb, string resource);
        Response Build(ApiGatewayArn apiGatewayArn, string principalId = null);
    }

    public class PolicyBuilder : IPolicyBuilder
    {
        private readonly string _policyVersion = "2012-10-17";
        private readonly string _apiAction = "execute-api:Invoke";
        private readonly Regex _pathRegex = new Regex("^[/.a-zA-Z0-9-\\*]+$");
        private readonly List<ApiGatewayArn> _allowMethodArns = new List<ApiGatewayArn>();
        private readonly List<ApiGatewayArn> _denyMethodArns = new List<ApiGatewayArn>();

        public void AllowAllMethods()
            => AddMethod(Effect.Allow, HttpVerb.All, "*");

        public void AllowMethod(HttpVerb verb, string resource)
            => AddMethod(Effect.Allow, verb, resource);

        public void DenyAllMethods()
            => AddMethod(Effect.Deny, HttpVerb.All, "*");

        public void DenyMethod(HttpVerb verb, string resource)
            => AddMethod(Effect.Deny, verb, resource);

        public Response Build(ApiGatewayArn apiGatewayArn, string principalId = null)
        {
            PopulateMethods(apiGatewayArn);
            var statements = GetStatements();

            return new Response
            {
                PrincipalID = principalId,
                PolicyDocument = new APIGatewayCustomAuthorizerPolicy
                {
                    Version = _policyVersion,
                    Statement = statements,
                }
            };
        }

        private void AddMethod(Effect effect, HttpVerb verb, string resource)
        {
            if (verb == null)
                throw new AuthPolicyBuilderException($"{nameof(verb)} cannot be null");
            if (resource == null)
                throw new AuthPolicyBuilderException($"{nameof(resource)} cannot be null");
            if (!_pathRegex.IsMatch(resource))
                throw new AuthPolicyBuilderException($"{nameof(resource)} is invalid: {resource}. Path should match {_pathRegex}");

            var cleanedResource = resource.First() == '/' ? resource.Substring(1) : resource;

            var arn = new ApiGatewayArn
            {
                Verb = verb.ToString(),
                Resource = cleanedResource,
            };

            switch (effect)
            {
                case Effect.Deny:
                    _denyMethodArns.Add(arn);
                    return;
                case Effect.Allow:
                    _allowMethodArns.Add(arn);
                    return;
            }
        }

        private void PopulateMethods(ApiGatewayArn apiGatewayArn)
        {
            populate(_allowMethodArns);
            populate(_denyMethodArns);

            void populate(List<ApiGatewayArn> methodArns)
            {
                foreach (var arn in methodArns)
                {
                    arn.Partition = apiGatewayArn.Partition;
                    arn.Service = apiGatewayArn.Service.DefaultTo("*");
                    arn.Region = apiGatewayArn.Region.DefaultTo("*");
                    arn.AwsAccountId = apiGatewayArn.AwsAccountId;
                    arn.RestApiId = apiGatewayArn.RestApiId.DefaultTo("*");
                    arn.Stage = apiGatewayArn.Stage.DefaultTo("*");
                }
            }
        }

        private List<IAMPolicyStatement> GetStatements()
        {
            var statements = new List<IAMPolicyStatement>();

            addStatements(Effect.Allow, _allowMethodArns);
            addStatements(Effect.Deny, _denyMethodArns);

            void addStatements(Effect effect, List<ApiGatewayArn> methodArns)
            {
                foreach (var arn in methodArns)
                {
                    statements.Add(new IAMPolicyStatement
                    {
                        Effect = effect.ToString(),
                        Resource = new HashSet<string> { arn.ToString() },
                        Action = new HashSet<string> { _apiAction },
                    });
                }
            }

            return statements;
        }
    }
}
