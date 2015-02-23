namespace IoT.Client.DotNet.Management
{
    public static class ManagementClientFactory
    {
        public static ManagementClient Create(string baseUri)
        {
            return new ManagementClient(baseUri, new RestConnection());
        }
    }
}
