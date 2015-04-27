namespace Thriot.Platform.PersistentConnections.Commands
{
    public class LoginCommand : Command
    {
        public string DeviceId { get; private set; }

        public string ApiKey { get; private set; }

        public LoginCommand(string parameters)
        {
            if (parameters == null)
                return;

            var parts = parameters.Split(' ');

            if (parts.Length == 2)
            {
                DeviceId = parts[0];
                ApiKey = parts[1];
                IsValid = true;
            }
        }
    }
}
