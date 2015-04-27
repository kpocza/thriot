namespace Thriot.Framework.Azure.DataAccess
{
    public class PartionKeyRowKeyPair
    {
        public string PartitionKey { get; private set; }
        public string RowKey { get; private set; }

        public PartionKeyRowKeyPair(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public static PartionKeyRowKeyPair CreateFromIdentity(string identity)
        {
            return new PartionKeyRowKeyPair(identity.Substring(0, 4), identity.Substring(4));
        }

        public static PartionKeyRowKeyPair CreateUnique(string id)
        {
            return new PartionKeyRowKeyPair(id, id);
        }

        public string GetCombination()
        {
            return PartitionKey + RowKey;
        }
    }
}
