using System.Collections.Generic;
using System.Linq;
using IoT.Platform.Model;
using IoT.Platform.Model.Messaging;

namespace IoT.Platform.PersistentConnections
{
    public class PersistentConnectionPeekWorker : PersistentConnectionWorker
    {
        public PersistentConnectionPeekWorker(PusherRegistry pusherRegistry,
            IMessagingOperations outgoingMessageReader, IBatchParameters batchParameters)
            : base(
                pusherRegistry, outgoingMessageReader, batchParameters.PersistentConnectionMessagePeekCollectionTime,
                batchParameters.PersistentConnectionMessagePeekCollectionBatch)
        {
        }

        protected override IPersistentConnection GetNextConnection()
        {
            return _pusherRegistry.GetPeekConnection();
        }

        protected override IDictionary<long, OutgoingMessageToStoreWithState> ProcessManyItems(IEnumerable<long> ids)
        {
            return _outgoingMessageReader.PeekMany(ids);
        }

        protected override void PostProcessConnections(IDictionary<long, IPersistentConnection> allConnections, IDictionary<long, OutgoingMessageToStoreWithState> firedConnections)
        {
            var firedConnectionIds = new HashSet<long>();

            foreach (var fcId in firedConnections.Where(fc => fc.Value.HasMessage).Select(fc => fc.Key))
            {
                firedConnectionIds.Add(fcId);
            }

            _pusherRegistry.SetAsCommitNeededConnections(
                allConnections.Where(c => firedConnectionIds.Contains(c.Key)).Select(c => c.Value).ToList());

            _pusherRegistry.RequeueAsPeekConnections(
                allConnections.Where(c => !firedConnectionIds.Contains(c.Key)).Select(c => c.Value).ToList());
        }
    }
}
