namespace IoT.Messaging.Services.Caching
{
    public class CacheIndex
    {
        public CacheIndex(long deviceId, int index)
        {
            DeviceId = deviceId;
            Index = index;
        }

        public long DeviceId { get; private set; }

        public int Index { get; private set; }

        public string CacheKey
        {
            get { return DeviceId + "_" + Index; }
        }

        public override int GetHashCode()
        {
            return CacheKey.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var otherCacheIndex = obj as CacheIndex;

            if (otherCacheIndex == null)
                return false;

            return CacheKey.Equals(otherCacheIndex.CacheKey);
        }
    }
}
