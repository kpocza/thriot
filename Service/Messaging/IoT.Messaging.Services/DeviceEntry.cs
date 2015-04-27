namespace Thriot.Messaging.Services
{
    public class DeviceEntry
    {
        public long Id { get; set; }

        public int DequeueIndex { get; set; }

        public int EnqueueIndex { get; set; }

        public bool Peek { get; set; }

        public int Version { get; set; }

        public DeviceEntry Clone()
        {
            return new DeviceEntry
            {
                Id = Id,
                DequeueIndex = DequeueIndex,
                EnqueueIndex = EnqueueIndex,
                Peek = Peek,
                Version = Version
            };
        }
    }
}
