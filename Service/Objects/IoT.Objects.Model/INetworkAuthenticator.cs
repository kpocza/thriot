namespace Thriot.Objects.Model
{
    public interface INetworkAuthenticator
    {
        bool Authenticate(AuthenticationParameters deviceAuthentication);
    }
}
