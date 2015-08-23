using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Thriot.Framework;
using Thriot.Plugins.Core;

namespace Thriot.Plugins.Azure
{
    public class QueueSendAdapter : IQueueSendAdapter
    {
        private CloudQueue _cloudQueue;

        public void Setup(IDictionary<string, string> parameters)
        {
            var connectionString = parameters["ConnectionString"];
            var queueName = parameters["QueueName"];
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var cloudQueueClient = storageAccount.CreateCloudQueueClient();
            _cloudQueue = cloudQueueClient.GetQueueReference(queueName);
        }

        public void Send(TelemetryData telemetryData)
        {
            var content = Serializers.ToJsonString(telemetryData);
            var message = new CloudQueueMessage(content);

            _cloudQueue.AddMessage(message);
        }

        public void Clear()
        {
            _cloudQueue.Clear();
        }
    }
}
