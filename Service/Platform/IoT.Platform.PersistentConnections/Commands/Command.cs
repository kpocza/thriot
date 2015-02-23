namespace IoT.Platform.PersistentConnections.Commands
{
    public abstract class Command
    {
        public bool IsValid { get; protected set; }
    }
}
