using System.Collections.Generic;
using Thriot.Platform.Model;
using Thriot.Platform.Model.Messaging;

namespace Thriot.Platform.PersistentConnections
{
    public class PersistentConnectionReceiveAndForgetWorker : PersistentConnectionWorker
    {
        public PersistentConnectionReceiveAndForgetWorker(PusherRegistry pusherRegistry,
            IMessagingOperations outgoingMessageReader, IBatchParameters batchParameters)
            : base(
                pusherRegistry, outgoingMessageReader,
                batchParameters.PersistentConnectionMessageReceiveAndForgetCollectionTime,
                batchParameters.PersistentConnectionMessageReceiveAndForgetCollectionBatch)
        {
        }

        protected override IPersistentConnection GetNextConnection()
        {
            return _pusherRegistry.GetReceiveAndForgetConnection();
        }

        protected override IDictionary<long, OutgoingMessageToStoreWithState> ProcessManyItems(IEnumerable<long> ids)
        {
            return _outgoingMessageReader.ReceiveAndForgetMany(ids);
        }

        protected override void PostProcessConnections(IDictionary<long, IPersistentConnection> allConnections, IDictionary<long, OutgoingMessageToStoreWithState> firedConnections)
        {
            // NOP
        }
    }
}
