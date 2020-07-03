using Xunit;

namespace ApiGatewayCustomAuthorizer.Tests
{
    public class ExtensionsTests
    {
        [Fact]
        public void DefaultTo_ReturnsDefaultValue_WhenStringIsNull()
        {
            var value = null as string;
            var defaultValue = "*";

            var actual = value.DefaultTo(defaultValue);

            Assert.Equal(defaultValue, actual);
        }

        [Fact]
        public void DefaultTo_ReturnsDefaultValue_WhenStringIsEmpty()
        {
            var value = "";
            var defaultValue = "*";

            var actual = value.DefaultTo(defaultValue);

            Assert.Equal(defaultValue, actual);
        }

        [Fact]
        public void DefaultTo_ReturnsDefaultValue_WhenStringIsOnlyWhiteSpace()
        {
            var value = " \t \n ";
            var defaultValue = "*";

            var actual = value.DefaultTo(defaultValue);

            Assert.Equal(defaultValue, actual);
        }

        [Fact]
        public void DefaultTo_ReturnsInitialValue_WhenStringHasNonWhiteSpace()
        {
            var value = "foobar";
            var defaultValue = "*";

            var actual = value.DefaultTo(defaultValue);

            Assert.Equal(value, actual);
        }
    }
}
