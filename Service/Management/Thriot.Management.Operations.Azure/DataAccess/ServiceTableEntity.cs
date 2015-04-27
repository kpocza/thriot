using System.Collections.Generic;
using Thriot.Framework;
using Thriot.Framework.Azure.DataAccess;
using Thriot.Management.Model;

namespace Thriot.Management.Operations.Azure.DataAccess
{
    public class ServiceTableEntity : PreparableTableEntity
    {
        public string Name { get; set; }

        public string CompanyId { get; set; }

        public string ApiKey { get; set; }

        public IList<Small> Networks { get; set; }

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

        public override void PrepareAfterLoad()
        {
            var json = BuildJsonFromByteArrays(
                () => NetworkData1,
                () => NetworkData2,
                () => NetworkData3,
                () => NetworkData4);

            Networks = new Wrapper<Small>(json).Entities;
        }

        public override void PrepareBeforeSave()
        {
            var json = new Wrapper<Small>(Networks).AsString();
            BuildByteArraysFromJson(json,
                (val) => NetworkData1 = val,
                (val) => NetworkData2 = val,
                (val) => NetworkData3 = val,
                (val) => NetworkData4 = val);
        }

        public ServiceTableEntity()
        {
            
        }

        public ServiceTableEntity(PartionKeyRowKeyPair partitionKeyRowKeyPair, string name, string companyId, string apiKey)
        {
            PartitionKey = partitionKeyRowKeyPair.PartitionKey;
            RowKey = partitionKeyRowKeyPair.RowKey;

            Name = name;
            CompanyId = companyId;
            ApiKey = apiKey;
            TelemetryDataSinkSettings = new TelemetryDataSinkSettings();
        }
    }
}
