namespace IoT.Platform.PersistentConnections.Commands
{
    public class SendToCommand : Command
    {
        public string DeviceId { get; private set; }

        public string Message { get; private set; }

        public SendToCommand(string parameters)
        {
            var indexOfSpace = parameters.IndexOf(' ');

            if (indexOfSpace != -1)
            {
                DeviceId = parameters.Substring(0, indexOfSpace);
                Message = parameters.Substring(indexOfSpace + 1);

                IsValid = true;
            }
        }
    }
}
