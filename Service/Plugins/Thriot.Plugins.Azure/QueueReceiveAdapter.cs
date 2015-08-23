using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Thriot.Framework;
using Thriot.Plugins.Core;

namespace Thriot.Plugins.Azure
{
    public class QueueReceiveAdapter : SerialQueueReceiveAdapter
    {
        private CloudQueue _cloudQueue;

        public override void Setup(IDictionary<string, string> parameters)
        {
            var connectionString = parameters["ConnectionString"];
            var queueName = parameters["QueueName"];
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var cloudQueueClient = storageAccount.CreateCloudQueueClient();
            _cloudQueue = cloudQueueClient.GetQueueReference(queueName);
        }

        protected override IEnumerable<QueueItem> DequeueItemsCore(int maxDequeueCount, int expirationMinutes)
        {
            var messages = _cloudQueue.GetMessages(maxDequeueCount, TimeSpan.FromMinutes(expirationMinutes), null, null);

            var items = new List<QueueItem>();
            foreach (var message in messages)
            {
                var telemetryData = Serializers.FromJsonString<TelemetryData>(message.AsString);
                items.Add(new QueueItem(message, telemetryData));
            }

            return items;
        }

        protected override void CommitItemsCore(IEnumerable<QueueItem> items)
        {
            foreach (var item in items)
            {
                _cloudQueue.DeleteMessage((CloudQueueMessage)item.NativeObject);
            }
        }
    }
}
