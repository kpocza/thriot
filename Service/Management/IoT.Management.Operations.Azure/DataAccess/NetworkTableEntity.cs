using System.Collections.Generic;
using IoT.Framework;
using IoT.Framework.Azure.DataAccess;
using IoT.Management.Model;

namespace IoT.Management.Operations.Azure.DataAccess
{
    public class NetworkTableEntity : PreparableTableEntity
    {
        public string Name { get; set; }

        public string ServiceId { get; set; }

        public string CompanyId { get; set; }

        public string ParentNetworkId { get; set; }

        public IList<Small> Networks { get; set; }

        public IList<Small> Devices { get; set; }

        public string NetworkKey { get; set; }

        public TelemetryDataSinkSettings TelemetryDataSinkSettings { get; set; }

        public string TelemetryDataSinkSettingsStorage
        {
            get { return Serializers.ToJsonString(TelemetryDataSinkSettings ?? new TelemetryDataSinkSettings()); }
            set { TelemetryDataSinkSettings = Serializers.FromJsonString<TelemetryDataSinkSettings>(value); }
        }

        public byte[] NetworkData1 { get; set; }
        public byte[] NetworkData2 { get; set; }
        public byte[] NetworkData3 { get; set; }
        public byte[] NetworkData4 { get; set; }


        public byte[] DeviceData1 { get; set; }
        public byte[] DeviceData2 { get; set; }
        public byte[] DeviceData3 { get; set; }
        public byte[] DeviceData4 { get; set; }

        public override void PrepareAfterLoad()
        {
            var jsonNetwork = BuildJsonFromByteArrays(
                () => NetworkData1,
                () => NetworkData2,
                () => NetworkData3,
                () => NetworkData4);

            Networks = new Wrapper<Small>(jsonNetwork).Entities;

            var jsonDevice = BuildJsonFromByteArrays(
                () => DeviceData1,
                () => DeviceData2,
                () => DeviceData3,
                () => DeviceData4);

            Devices = new Wrapper<Small>(jsonDevice).Entities;
        }

        public override void PrepareBeforeSave()
        {
            var jsonNetwork = new Wrapper<Small>(Networks).AsString();
            BuildByteArraysFromJson(jsonNetwork,
                (val) => NetworkData1 = val,
                (val) => NetworkData2 = val,
                (val) => NetworkData3 = val,
                (val) => NetworkData4 = val);

            var jsonDevice = new Wrapper<Small>(Devices).AsString();
            BuildByteArraysFromJson(jsonDevice,
                (val) => DeviceData1 = val,
                (val) => DeviceData2 = val,
                (val) => DeviceData3 = val,
                (val) => DeviceData4 = val);
        }

        public NetworkTableEntity()
        {
            
        }

        public NetworkTableEntity(PartionKeyRowKeyPair partitionKeyRowKeyPair, string name, string parentNetworkId, string serviceId, string companyId, string networkKey)
        {
            PartitionKey = partitionKeyRowKeyPair.PartitionKey;
            RowKey = partitionKeyRowKeyPair.RowKey;

            Name = name;
            ParentNetworkId = parentNetworkId;
            ServiceId = serviceId;
            CompanyId = companyId;
            NetworkKey = networkKey;
            TelemetryDataSinkSettings = new TelemetryDataSinkSettings();
        }
    }
}
