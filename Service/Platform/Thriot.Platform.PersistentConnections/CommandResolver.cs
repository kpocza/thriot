using Thriot.Platform.PersistentConnections.Commands;

namespace Thriot.Platform.PersistentConnections
{
    public class CommandResolver
    {
        private readonly string _message;

        public CommandResolver(string message)
        {
            _message = message;
        }

        public Command GetCommand()
        {
            if (_message == null)
                return null;

            var commandParts = new CommandParts(_message);

            switch (commandParts.Operation)
            {
                case "login":
                    return new LoginCommand(commandParts.Parameters);
                case "telemetrydata":
                    return new TelemetryDataCommand(commandParts.Parameters);
                case "subscribe":
                    return new SubscribeCommand(commandParts.Parameters);
                case "unsubscribe":
                    return new UnsubscribeCommand();
                case "commit":
                    return new CommitCommand();
                case "heartbeat":
                    return new HeartbeatCommand();
                case "close":
                    return new CloseCommand();
                case "sendto":
                    return new SendToCommand(commandParts.Parameters);
            }

            return null;
        }

        public class CommandParts
        {
            public string Operation { get; private set; }

            public string Parameters { get; private set; }

            public CommandParts(string message)
            {
                var firstPart = message.IndexOf(' ');

                if (firstPart == -1)
                {
                    Operation = message;
                    Parameters = null;
                }
                else
                {
                    Operation = message.Substring(0, firstPart);
                    Parameters = message.Substring(firstPart + 1);
                }
            }
        }
    }
}
