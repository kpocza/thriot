using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Thriot.Client.DotNet.Platform
{
    /// <summary>
    /// Represents an occasionally connected client. There is no persistent connection, every operation is a standalone REST API request.
    /// </summary>
    public class OccasionallyConnectionClient
    {
        private readonly IRestConnection _restConnection;

        /// <summary>
        /// Create new instance of an occasionally connected client
        /// </summary>
        /// <param name="baseUrl">Base API url</param>
        /// <param name="deviceId">Unique device identifier</param>
        /// <param name="apiKey">Api key of the device, enclosing network or enclosing service</param>
        public OccasionallyConnectionClient(string baseUrl, string deviceId, string apiKey)
            : this(baseUrl, deviceId, apiKey, new RestConnection())
        {
        }

        private OccasionallyConnectionClient(string baseUrl, string deviceId, string apiKey, IRestConnection restConnection)
        {
            _restConnection = restConnection;
            _restConnection.Setup(baseUrl, new Dictionary<string, string>
            {
                {"X-DeviceId", deviceId},
                {"X-ApiKey", apiKey}
            });
        }

        /// <summary>
        /// Record telemetry data.
        /// 
        /// Send POST request to the APIROOT/telemetry Url
        /// </summary>
        /// <param name="message">Telemetry data in JSON format with maximum length of 1024 characters</param>
        /// <exception cref="WebException">This exception indicates some service level error. Please refer to the HTTP error code for more information</exception>
        public void RecordTelmetryData(string message)
        {
            _restConnection.Post("telemetry", message);
        }

        /// <summary>
        /// Send message to a device. Only to devices in the same network can messages be sent.
        /// 
        /// Send POST request to the APIROOT/message/deviceId Url
        /// </summary>
        /// <param name="deviceId">Target device id</param>
        /// <param name="message">Message with maximum length of 512 bytes</param>
        /// <exception cref="WebException">This exception indicates some service level error. Please refer to the HTTP error code for more information</exception>
        public void SendMessageTo(string deviceId, string message)
        {
            string payload = "\"" + Convert.ToBase64String(Encoding.UTF8.GetBytes(message)) + "\"";

            _restConnection.Post("messages/sendto/" + deviceId, payload);
        }

        
        /// <summary>
        /// Receives a message sent to the current device. After receiving the message will be removed from the queue. In case of any error the message cannot be rereceived.
        /// This method implements QoS 0-like reliability from the reveiver's perspective.
        /// 
        /// Send GET request to the APIROOT/message/forget Url
        /// </summary>
        /// <returns></returns>
        /// <exception cref="WebException">This exception indicates some service level error. Please refer to the HTTP error code for more information</exception>
        public PushedMessage ReceiveAndForgetMessage()
        {
            var result = _restConnection.Get("messages/forget");

            if (result == "null")
                return null;

            return ConvertToPushedMessage(result);
        }

        /// <summary>
        /// Receives a message sent to the current device. After receiving you should call the <see cref="CommitMessage"/> method that will remove the message from the queue. 
        /// In case of any error the message can be rereceived until the <see cref="CommitMessage"/> is called.
        /// This method implements QoS 1-like reliability from the reveiver's perspective.
        ///  
        /// Send GET request to the APIROOT/message/peek Url
        /// </summary>
        /// <returns></returns>
        /// <exception cref="WebException">This exception indicates some service level error. Please refer to the HTTP error code for more information</exception>
        public PushedMessage PeekMessage()
        {
            var result = _restConnection.Get("messages/peek");

            if (result == "null")
                return null;

            return ConvertToPushedMessage(result);
        }

        /// <summary>
        /// Informs the service that the message received by the <see cref="PeekMessage"/> method was successfully processed.
        /// This method implements QoS 1-like reliability from the reveiver's perspective.
        /// 
        /// Send POST request to the APIROOT/message/commit Url
        /// </summary>
        /// <exception cref="WebException">This exception indicates some service level error. Please refer to the HTTP error code for more information</exception>
        public void CommitMessage()
        {
            _restConnection.Post("messages/commit", "");
        }

        private static PushedMessage ConvertToPushedMessage(string outgoingMessageStr)
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof (PushedMessageDto));

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(outgoingMessageStr)))
            {
                ms.Position = 0;
                var outgoingMessage = (PushedMessageDto) jsonSerializer.ReadObject(ms);

                return new PushedMessage(outgoingMessage.MessageId, outgoingMessage.Timestamp, outgoingMessage.Payload, outgoingMessage.SenderDeviceId);
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

            [DataMember]
            public string SenderDeviceId { get; set; }
        }
    }
}
