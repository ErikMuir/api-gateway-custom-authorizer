using System;
using Xunit;

namespace ApiGatewayCustomAuthorizer.Tests
{
    public class ApiGatewayArnTests
    {
        private const string _methodArn = "arn:partition:service:region:awsAccountId:restApiId/stage/verb";

        [Fact]
        public void Parse_Returns_ApiGatewayArn()
        {
            Assert.NotNull(ApiGatewayArn.Parse(_methodArn));
        }

        [Fact]
        public void Parse_Sets_Partition()
        {
            Assert.Equal("partition", ApiGatewayArn.Parse(_methodArn).Partition);
        }

        [Fact]
        public void Parse_Sets_Service()
        {
            Assert.Equal("service", ApiGatewayArn.Parse(_methodArn).Service);
        }

        [Fact]
        public void Parse_Sets_Region()
        {
            Assert.Equal("region", ApiGatewayArn.Parse(_methodArn).Region);
        }

        [Fact]
        public void Parse_Sets_AwsAccountId()
        {
            Assert.Equal("awsAccountId", ApiGatewayArn.Parse(_methodArn).AwsAccountId);
        }

        [Fact]
        public void Parse_Sets_RestApiId()
        {
            Assert.Equal("restApiId", ApiGatewayArn.Parse(_methodArn).RestApiId);
        }

        [Fact]
        public void Parse_Sets_Stage()
        {
            Assert.Equal("stage", ApiGatewayArn.Parse(_methodArn).Stage);
        }

        [Fact]
        public void Parse_Sets_Verb()
        {
            Assert.Equal("verb", ApiGatewayArn.Parse(_methodArn).Verb);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("resource")]
        [InlineData("resource/with/slashes")]
        public void Parse_Sets_Resource(string resource)
        {
            var methodArn = resource == null ? _methodArn : _methodArn + "/" + resource;

            Assert.Equal(resource, ApiGatewayArn.Parse(methodArn).Resource);
        }

        [Fact]
        public void Parse_Throws_WhenArnIsInvalid()
        {
            var invalidArn = "this-is-an-invalid-arn";

            var actual = Record.Exception(() => ApiGatewayArn.Parse(invalidArn));

            Assert.NotNull(actual);
            Assert.IsAssignableFrom<Exception>(actual);
        }

        [Fact]
        public void ToString_ReturnsExpectedString()
        {
            var expected = $"{_methodArn}/";

            var actual = ApiGatewayArn.Parse(_methodArn).ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TryParse_WhenArnIsValid()
        {
            var expected = ApiGatewayArn.Parse(_methodArn);

            Assert.True(ApiGatewayArn.TryParse(_methodArn, out var actual));
            Assert.Equal(expected.ToString(), actual.ToString());
        }
    }
}
