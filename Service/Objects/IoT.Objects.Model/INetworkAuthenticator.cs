namespace IoT.Objects.Model
{
    public interface INetworkAuthenticator
    {
        bool Authenticate(AuthenticationParameters deviceAuthentication);
    }
}
