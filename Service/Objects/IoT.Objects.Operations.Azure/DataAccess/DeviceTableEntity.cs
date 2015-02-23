using IoT.Framework.Azure.DataAccess;

namespace IoT.Objects.Operations.Azure.DataAccess
{
    public class DeviceTableEntity : PreparableTableEntity
    {
        public string Name { get; set; }

        public string ServiceId { get; set; }

        public string CompanyId { get; set; }

        public string NetworkId { get; set; }

        public string DeviceKey { get; set; }

        public long NumericId { get; set; }
    }
}
