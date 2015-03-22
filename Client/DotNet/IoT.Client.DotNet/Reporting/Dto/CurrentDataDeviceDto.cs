namespace IoT.Client.DotNet.Reporting
{
    /// <summary>
    /// Latest telemetry data for a given device
    /// </summary>
    public class CurrentDataDeviceDto
    {
        /// <summary>
        /// Unique identifier of the device
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// Name of the device
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// UNIX timestamp of the latest data recording (elapsed seconds since 1970.1.1)
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// Latest payload
        /// </summary>
        public string Payload { get; set; }
    }
}
