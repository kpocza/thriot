namespace Thriot.Objects.Model
{
    public class AuthenticationParameters
    {
        public AuthenticationParameters(string id, string apiKey)
        {
            Id = id;
            ApiKey = apiKey;
        }

        public string Id { get; private set; }

        public string ApiKey { get; private set; }
    }
}
