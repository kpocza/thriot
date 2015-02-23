namespace IoT.Client.DotNet.Platform
{
    public static class PersistentConnectionClientFactory
    {
        public static PersistentConnectionClient Create()
        {
            return new PersistentConnectionClient();
        }
    }
}
