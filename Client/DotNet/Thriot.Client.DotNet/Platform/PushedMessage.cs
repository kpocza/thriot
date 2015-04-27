using System;

namespace Thriot.Client.DotNet.Platform
{
    /// <summary>
    /// A message that was received from other device
    /// </summary>
    public class PushedMessage
    {
        /// <summary>
        /// Incremental identifier of the message. Ids are unique by device not globally.
        /// </summary>
        public int MessageId { get; private set; }

        /// <summary>
        /// When the message was sent
        /// </summary>
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// Message payload
        /// </summary>
        public string Payload { get; private set; }

        /// <summary>
        /// Constructs a new instance
        /// </summary>
        /// <param name="messageId">Unique id</param>
        /// <param name="timestamp">Timestamp</param>
        /// <param name="payload">Message content</param>
        public PushedMessage(int messageId, long timestamp, string payload)
        {
            MessageId = messageId;
            Timestamp = new DateTime(1970, 1, 1) + TimeSpan.FromSeconds(timestamp);
            Payload = payload;
        }
    }
}
