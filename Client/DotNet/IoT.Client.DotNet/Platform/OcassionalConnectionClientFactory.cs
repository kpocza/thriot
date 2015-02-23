namespace IoT.Client.DotNet.Platform
{
    public static class OcassionalConnectionClientFactory
    {
        public static OcassionalConnectionClient Create(string baseUrl, string deviceId, string apiKey)
        {
            return new OcassionalConnectionClient(baseUrl, deviceId, apiKey, new RestConnection());
        }
    }
}
