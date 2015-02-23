using System.Collections.Generic;
using IoT.Platform.Model;
using IoT.Platform.Model.Messaging;

namespace IoT.Platform.PersistentConnections
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
