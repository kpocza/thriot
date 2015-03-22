namespace IoT.Client.DotNet.Platform
{
    /// <summary>
    /// Represents the subscription type
    /// </summary>
    public enum SubscriptionType
    {
        /// <summary>
        /// This is a QoS 0-level reliability from the receivers perspective
        /// </summary>
        ReceiveAndForget = 0,
        /// <summary>
        /// This is a QoS 1-level reliability from the receivers perspective
        /// </summary>
        PeekAndCommit = 1
    }
}