namespace IoT.Objects.Model
{
    public interface IDeviceAuthenticator
    {
        bool Authenticate(AuthenticationParameters deviceAuthentication);
    }
}
