namespace IoT.Client.DotNet.Reporting
{
    /// <summary>
    /// Time series data entry
    /// </summary>
    public class TimeSeriesRowDto
    {
        /// <summary>
        /// Record time of the time series entry in seconds elapsed since 19701.1.1 (UNIX timestamp)
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// Data payload
        /// </summary>
        public string Payload { get; set; }
    }
}
