namespace IoT.Client.DotNet.Reporting
{
    public static class ReportingClientFactory
    {
        public static ReportingClient Create(string baseUrl)
        {
            return new ReportingClient(baseUrl, new RestConnection());
        }
    }
}
