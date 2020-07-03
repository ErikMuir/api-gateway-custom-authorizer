namespace ApiGatewayCustomAuthorizer
{
    public sealed class HttpVerb
    {
        private readonly string _verb;

        private HttpVerb(string verb)
        {
            _verb = verb;
        }

        public override string ToString()
        {
            return _verb;
        }

        public static HttpVerb Get => new HttpVerb("GET");
        public static HttpVerb Post => new HttpVerb("POST");
        public static HttpVerb Put => new HttpVerb("PUT");
        public static HttpVerb Patch => new HttpVerb("PATCH");
        public static HttpVerb Head => new HttpVerb("HEAD");
        public static HttpVerb Delete => new HttpVerb("DELETE");
        public static HttpVerb Options => new HttpVerb("OPTIONS");
        public static HttpVerb All => new HttpVerb("*");
    }
}
