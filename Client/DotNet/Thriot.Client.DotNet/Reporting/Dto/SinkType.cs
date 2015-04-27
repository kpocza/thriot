namespace Thriot.Client.DotNet.Reporting
{
    /// <summary>
    /// Sink types
    /// </summary>
    public enum SinkType
    {
        /// <summary>
        /// Sink that always has only the latest recorded data
        /// </summary>
        CurrentData = 1,

        /// <summary>
        /// Sink that stores telemetry data history
        /// </summary>
        TimeSeries = 2
    }
}