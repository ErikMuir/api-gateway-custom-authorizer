using Xunit;

namespace ApiGatewayCustomAuthorizer.Tests
{
    public class HttpVerbTests
    {
        [Fact]
        public void HttpVerb_Get()
        {
            Assert.Equal("GET", HttpVerb.Get.ToString());
        }

        [Fact]
        public void HttpVerb_Post()
        {
            Assert.Equal("POST", HttpVerb.Post.ToString());
        }

        [Fact]
        public void HttpVerb_Put()
        {
            Assert.Equal("PUT", HttpVerb.Put.ToString());
        }

        [Fact]
        public void HttpVerb_Patch()
        {
            Assert.Equal("PATCH", HttpVerb.Patch.ToString());
        }

        [Fact]
        public void HttpVerb_Head()
        {
            Assert.Equal("HEAD", HttpVerb.Head.ToString());
        }

        [Fact]
        public void HttpVerb_Delete()
        {
            Assert.Equal("DELETE", HttpVerb.Delete.ToString());
        }

        [Fact]
        public void HttpVerb_Options()
        {
            Assert.Equal("OPTIONS", HttpVerb.Options.ToString());
        }

        [Fact]
        public void HttpVerb_All()
        {
            Assert.Equal("*", HttpVerb.All.ToString());
        }
    }
}
