using System.Text.Json;

namespace ApiGatewayCustomAuthorizer
{
    public class JsonConfig
    {
        public static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
        {
            IgnoreNullValues = true,
        };
    }
}
