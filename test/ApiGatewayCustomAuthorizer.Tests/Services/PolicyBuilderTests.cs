using System.Linq;
using Xunit;

namespace ApiGatewayCustomAuthorizer.Tests
{
    public class PolicyBuilderTests
    {
        private const string StandardArn = "arn:partition:service:region:awsAccountId:restApiId/stage/verb";
        private readonly ApiGatewayArn _standardArn;
        private readonly PolicyBuilder _testObject;

        public PolicyBuilderTests()
        {
            _standardArn = ApiGatewayArn.Parse(StandardArn);
            _testObject = new PolicyBuilder();
        }

        [Fact]
        public void AllowMethod_Throws_WhenHttpVerbIsNull()
        {
            var ex = Record.Exception(() => _testObject.AllowMethod(null, ""));

            Assert.NotNull(ex);
            Assert.IsType<AuthPolicyBuilderException>(ex);
            Assert.Contains("verb cannot be null", ex.Message);
        }

        [Fact]
        public void AllowMethod_Throws_WhenResourceIsNull()
        {
            var ex = Record.Exception(() => _testObject.AllowMethod(HttpVerb.Get, null));

            Assert.NotNull(ex);
            Assert.IsType<AuthPolicyBuilderException>(ex);
            Assert.Contains("resource cannot be null", ex.Message);
        }

        [Fact]
        public void AllowMethod_Throws_WhenResourceIsInvalid()
        {
            var ex = Record.Exception(() => _testObject.AllowMethod(HttpVerb.Get, "%^&"));

            Assert.NotNull(ex);
            Assert.IsType<AuthPolicyBuilderException>(ex);
            Assert.Contains("resource is invalid", ex.Message);
        }

        [Fact]
        public void DenyMethod_Throws_WhenHttpVerbIsNull()
        {
            var ex = Record.Exception(() => _testObject.DenyMethod(null, ""));

            Assert.NotNull(ex);
            Assert.IsType<AuthPolicyBuilderException>(ex);
            Assert.Contains("verb cannot be null", ex.Message);
        }

        [Fact]
        public void DenyMethod_Throws_WhenResourceIsNull()
        {
            var ex = Record.Exception(() => _testObject.DenyMethod(HttpVerb.Get, null));

            Assert.NotNull(ex);
            Assert.IsType<AuthPolicyBuilderException>(ex);
            Assert.Contains("resource cannot be null", ex.Message);
        }

        [Fact]
        public void DenyMethod_Throws_WhenResourceIsInvalid()
        {
            var ex = Record.Exception(() => _testObject.DenyMethod(HttpVerb.Get, "%^&"));

            Assert.NotNull(ex);
            Assert.IsType<AuthPolicyBuilderException>(ex);
            Assert.Contains("resource is invalid", ex.Message);
        }

        [Fact]
        public void Build_ReturnsAuthResponse()
        {
            var actual = _testObject.Build(_standardArn);

            Assert.NotNull(actual);
        }

        [Fact]
        public void Build_SetsPrincipalID()
        {
            var principalId = "fake-principal-id";

            var actual = _testObject.Build(_standardArn, principalId);

            Assert.Equal(principalId, actual.PrincipalID);
        }

        [Fact]
        public void Build_SetsPolicyDocument()
        {
            var actual = _testObject.Build(_standardArn);

            Assert.NotNull(actual.PolicyDocument);
        }

        [Fact]
        public void Build_SetsVersion()
        {
            var actual = _testObject.Build(_standardArn);

            Assert.Equal("2012-10-17", actual.PolicyDocument.Version);
        }

        [Fact]
        public void Build_SetsStatement()
        {
            var actual = _testObject.Build(_standardArn);

            Assert.NotNull(actual.PolicyDocument.Statement);
        }

        [Fact]
        public void Build_ReturnsEmptyStatement_WhenNoMethodsWereAdded()
        {
            var actual = _testObject.Build(_standardArn);

            Assert.Empty(actual.PolicyDocument.Statement);
        }

        [Fact]
        public void Build_SetsStatement_WhenAllowAllMethodsWasCalled()
        {
            _testObject.AllowAllMethods();

            var actual = _testObject.Build(_standardArn);

            Assert.Single(actual.PolicyDocument.Statement);

            var statement = actual.PolicyDocument.Statement.Single();
            Assert.Equal("Allow", statement.Effect);
            Assert.Single(statement.Action);
            Assert.Contains("execute-api:Invoke", statement.Action);
            Assert.Single(statement.Resource);
            Assert.Contains($"arn:partition:service:region:awsAccountId:restApiId/stage/*/*", statement.Resource);
        }

        [Fact]
        public void Build_SetsStatement_WhenDenyAllMethodsWasCalled()
        {
            _testObject.DenyAllMethods();

            var actual = _testObject.Build(_standardArn);

            Assert.Single(actual.PolicyDocument.Statement);

            var statement = actual.PolicyDocument.Statement.Single();
            Assert.Equal("Deny", statement.Effect);
            Assert.Single(statement.Action);
            Assert.Contains("execute-api:Invoke", statement.Action);
            Assert.Single(statement.Resource);
            Assert.Contains($"arn:partition:service:region:awsAccountId:restApiId/stage/*/*", statement.Resource);
        }

        [Fact]
        public void Build_SetsStatement_WhenMultipleSpecificMethodsWereAdded()
        {
            var allowVerb = HttpVerb.Get;
            var denyVerb = HttpVerb.Delete;
            var resourceToAllow = "resource-to-allow";
            var resourceToDeny = "resource-to-deny";

            _testObject.AllowMethod(allowVerb, resourceToAllow);
            _testObject.DenyMethod(denyVerb, resourceToDeny);

            var actual = _testObject.Build(_standardArn);

            Assert.Equal(2, actual.PolicyDocument.Statement.Count);
            Assert.Single(actual.PolicyDocument.Statement.Where(s => s.Effect == "Allow"));
            Assert.Single(actual.PolicyDocument.Statement.Where(s => s.Effect == "Deny"));

            var allowStatement = actual.PolicyDocument.Statement.Where(s => s.Effect == "Allow").Single();
            Assert.Contains("execute-api:Invoke", allowStatement.Action);
            Assert.Single(allowStatement.Resource);
            Assert.Contains($"arn:partition:service:region:awsAccountId:restApiId/stage/GET/{resourceToAllow}", allowStatement.Resource);

            var denyStatement = actual.PolicyDocument.Statement.Where(s => s.Effect == "Deny").Single();
            Assert.Contains("execute-api:Invoke", denyStatement.Action);
            Assert.Single(denyStatement.Resource);
            Assert.Contains($"arn:partition:service:region:awsAccountId:restApiId/stage/DELETE/{resourceToDeny}", denyStatement.Resource);
        }

        [Fact]
        public void Build_SetsService_WhenNoServiceProvided()
        {
            var apiGatewayArn = ApiGatewayArn.Parse("arn:partition::region:awsAccountId:restApiId/stage/verb");
            _testObject.AllowAllMethods();

            var actual = _testObject.Build(apiGatewayArn);

            var statement = actual.PolicyDocument.Statement.Single();
            Assert.Contains($"arn:partition:*:region:awsAccountId:restApiId/stage/*/*", statement.Resource);
        }

        [Fact]
        public void Build_SetsRegion_WhenNoRegionProvided()
        {
            var apiGatewayArn = ApiGatewayArn.Parse("arn:partition:service::awsAccountId:restApiId/stage/verb");
            _testObject.AllowAllMethods();

            var actual = _testObject.Build(apiGatewayArn);

            var statement = actual.PolicyDocument.Statement.Single();
            Assert.Contains($"arn:partition:service:*:awsAccountId:restApiId/stage/*/*", statement.Resource);
        }

        [Fact]
        public void Build_SetsRestApiId_WhenNoRestApiIdProvided()
        {
            var apiGatewayArn = ApiGatewayArn.Parse("arn:partition:service:region:awsAccountId:/stage/verb");
            _testObject.AllowAllMethods();

            var actual = _testObject.Build(apiGatewayArn);

            var statement = actual.PolicyDocument.Statement.Single();
            Assert.Contains($"arn:partition:service:region:awsAccountId:*/stage/*/*", statement.Resource);
        }

        [Fact]
        public void Build_SetsStage_WhenNoStageProvided()
        {
            var apiGatewayArn = ApiGatewayArn.Parse("arn:partition:service:region:awsAccountId:restApiId//verb");
            _testObject.AllowAllMethods();

            var actual = _testObject.Build(apiGatewayArn);

            var statement = actual.PolicyDocument.Statement.Single();
            Assert.Contains($"arn:partition:service:region:awsAccountId:restApiId/*/*/*", statement.Resource);
        }
    }
}
