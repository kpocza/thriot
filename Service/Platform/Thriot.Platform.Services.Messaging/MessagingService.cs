using System;
using System.Text;
using Thriot.Framework.Exceptions;
using Thriot.Objects.Model.Operations;
using Thriot.Platform.Model.Messaging;

namespace Thriot.Platform.Services.Messaging
{
    public class MessagingService
    {
        private readonly IMessagingOperations _messagingOperations;
        private readonly IDeviceOperations _deviceOperations;

        private const int OutgoingMessageSizeLimit = 512;

        public MessagingService(IMessagingOperations messagingOperations, IDeviceOperations deviceOperations)
        {
            _messagingOperations = messagingOperations;
            _deviceOperations = deviceOperations;
        }

        public OutgoingState RecordOutgoingMessage(string senderDeviceId, string deviceId, string payload)
        {
            if (deviceId == null || senderDeviceId == null)
                throw new ForbiddenException();

            if (payload == null)
                throw new ArgumentNullException();

            if (payload.Length > OutgoingMessageSizeLimit)
                throw new ArgumentOutOfRangeException("payload");

            var messageBytes = Encoding.UTF8.GetBytes(payload);

            if(messageBytes.Length > OutgoingMessageSizeLimit)
                throw new ArgumentOutOfRangeException("payload");

            var device = _deviceOperations.Get(deviceId);
            
            if (senderDeviceId != deviceId)
            {
                var senderDevice = _deviceOperations.Get(senderDeviceId);
                
                if(device.NetworkId!= senderDevice.NetworkId)
                    throw new ForbiddenException();
            }

            var outgoingMessage = new OutgoingMessageToStore(device.NumericId, messageBytes, -1, DateTime.UtcNow, senderDeviceId);

            return _messagingOperations.Record(outgoingMessage);
        }

        public OutgoingMessageToSendWithState ReceiveAndForgetOutgoingMessage(string deviceId)
        {
            if (deviceId == null)
                throw new ForbiddenException();

            var device = _deviceOperations.Get(deviceId);

            var outgoingMessage = _messagingOperations.ReceiveAndForget(device.NumericId);

            if (outgoingMessage.State!= OutgoingState.Ok)
                return new OutgoingMessageToSendWithState(null, outgoingMessage.State);

            if (outgoingMessage.Message != null)
            {
                var payload = Encoding.UTF8.GetString(outgoingMessage.Message.Payload);
                return new OutgoingMessageToSendWithState(new OutgoingMessageToSend(deviceId, payload, outgoingMessage.Message.Time, outgoingMessage.Message.MessageId, outgoingMessage.Message.SenderDeviceId), OutgoingState.Ok);
            }

            return new OutgoingMessageToSendWithState(null, OutgoingState.Ok);
        }

        public OutgoingMessageToSendWithState Peek(string deviceId)
        {
            if (deviceId == null)
                throw new ForbiddenException();

            var device = _deviceOperations.Get(deviceId);

            var outgoingMessage = _messagingOperations.Peek(device.NumericId);

            if (outgoingMessage.State != OutgoingState.Ok)
                return new OutgoingMessageToSendWithState(null, outgoingMessage.State);

            if (outgoingMessage.Message != null)
            {
                var payload = Encoding.UTF8.GetString(outgoingMessage.Message.Payload);
                return new OutgoingMessageToSendWithState(new OutgoingMessageToSend(deviceId, payload, outgoingMessage.Message.Time, outgoingMessage.Message.MessageId, outgoingMessage.Message.SenderDeviceId), OutgoingState.Ok);
            }

            return new OutgoingMessageToSendWithState(null, OutgoingState.Ok);
        }

        public OutgoingState Commit(string deviceId)
        {
            if (deviceId == null)
                throw new ForbiddenException();

            var device = _deviceOperations.Get(deviceId);

            return _messagingOperations.Commit(device.NumericId);
        }
    }
}
