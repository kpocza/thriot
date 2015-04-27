namespace Thriot.Client.DotNet.Management
{
    public abstract class SpecificManagementClient
    {
        protected readonly IRestConnection RestConnection;

        protected SpecificManagementClient(IRestConnection restConnection)
        {
            RestConnection = restConnection;
        }
    }
}
