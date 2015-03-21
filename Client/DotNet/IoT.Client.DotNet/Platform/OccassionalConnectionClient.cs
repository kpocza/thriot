using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace IoT.Client.DotNet.Platform
{
    public class OccassionalConnectionClient
    {
        private readonly IRestConnection _restConnection;

        public OccassionalConnectionClient(string baseUrl, string deviceId, string apiKey) : this(baseUrl, deviceId, apiKey, new RestConnection())
        {
        }

        private OccassionalConnectionClient(string baseUrl, string deviceId, string apiKey, IRestConnection restConnection)
        {
            _restConnection = restConnection;
            _restConnection.Setup(baseUrl, new Dictionary<string, string>
            {
                {"X-DeviceId", deviceId},
                {"X-ApiKey", apiKey}
            });
        }

        public void RecordTelmetryData(string message)
        {
            _restConnection.Post("telemetry", message);
        }

        public void SendMessageTo(string deviceId, string message)
        {
            string payload = "\"" + Convert.ToBase64String(Encoding.UTF8.GetBytes(message)) + "\"";

            _restConnection.Post("messages/sendto/" + deviceId, payload);
        }

        public PushedMessage ReceiveAndForgetMessage()
        {
            var result = _restConnection.Get("messages/forget");

            if (result == "null")
                return null;

            return ConvertToPushedMessage(result);
        }

        public PushedMessage PeekMessage()
        {
            var result = _restConnection.Get("messages/peek");

            if (result == "null")
                return null;

            return ConvertToPushedMessage(result);
        }

        public void CommitMessage()
        {
            _restConnection.Post("messages/commit", "");
        }

        private static PushedMessage ConvertToPushedMessage(string outgoingMessageStr)
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(PushedMessageDto));

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(outgoingMessageStr)))
            {
                ms.Position = 0;
                var outgoingMessage = (PushedMessageDto)jsonSerializer.ReadObject(ms);

                return new PushedMessage(outgoingMessage.MessageId, outgoingMessage.Timestamp, outgoingMessage.Payload);
            }
        }

        [DataContract]
        public class PushedMessageDto
        {
            [DataMember]
            public string Payload { get; set; }

            [DataMember]
            public long Timestamp { get; set; }

            [DataMember]
            public int MessageId { get; set; }
        }
    }
}
