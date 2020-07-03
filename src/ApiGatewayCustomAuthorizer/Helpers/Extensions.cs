namespace ApiGatewayCustomAuthorizer
{
    public static class Extensions
    {
        public static string DefaultTo(this string val, string defaultVal)
        {
            return string.IsNullOrWhiteSpace(val) ? defaultVal : val;
        }
    }
}
