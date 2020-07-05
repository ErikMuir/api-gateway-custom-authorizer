using Xunit;

namespace ApiGatewayCustomAuthorizer.Tests
{
    public class ExtensionsTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("*")]
        [InlineData("foobar")]
        public void DefaultTo_ReturnsDefaultValue_WhenStringIsNull(string defaultValue)
        {
            var value = null as string;

            var actual = value.DefaultTo(defaultValue);

            Assert.Equal(defaultValue, actual);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("*")]
        [InlineData("foobar")]
        public void DefaultTo_ReturnsDefaultValue_WhenStringIsEmpty(string defaultValue)
        {
            var value = "";

            var actual = value.DefaultTo(defaultValue);

            Assert.Equal(defaultValue, actual);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("*")]
        [InlineData("foobar")]
        public void DefaultTo_ReturnsDefaultValue_WhenStringIsOnlyWhiteSpace(string defaultValue)
        {
            var value = " \t \n ";

            var actual = value.DefaultTo(defaultValue);

            Assert.Equal(defaultValue, actual);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("*")]
        [InlineData("foobar")]
        public void DefaultTo_ReturnsInitialValue_WhenStringHasNonWhiteSpace(string defaultValue)
        {
            var value = "foobar";

            var actual = value.DefaultTo(defaultValue);

            Assert.Equal(value, actual);
        }
    }
}
