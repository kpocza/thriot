namespace IoT.Client.DotNet.Reporting
{
    public class CurrentDataDeviceDto
    {
        public string DeviceId { get; set; }

        public string Name { get; set; }

        public long Timestamp { get; set; }

        public string Payload { get; set; }
    }
}
