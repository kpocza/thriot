using System.Collections.Generic;
using IoT.Framework;
using IoT.Framework.Azure.DataAccess;
using IoT.Management.Model;

namespace IoT.Management.Operations.Azure.DataAccess
{
    public class CompanyTableEntity : PreparableTableEntity
    {
        public string Name { get; set; }

        public IList<SmallUser> Users { get; set; }

        public IList<Small> Services { get; set; }

        public TelemetryDataSinkSettings TelemetryDataSinkSettings { get; set; }

        public string UsersStorage
        {
            get { return new Wrapper<SmallUser>(Users).AsString(); }
            set { Users = new Wrapper<SmallUser>(value).Entities; }
        }

        public string TelemetryDataSinkSettingsStorage
        {
            get { return Serializers.ToJsonString(TelemetryDataSinkSettings ?? new TelemetryDataSinkSettings()); }
            set { TelemetryDataSinkSettings = Serializers.FromJsonString<TelemetryDataSinkSettings>(value); }
        }

        public byte[] ServiceData1 { get; set; }
        public byte[] ServiceData2 { get; set; }
        public byte[] ServiceData3 { get; set; }
        public byte[] ServiceData4 { get; set; }

        public override void PrepareAfterLoad()
        {
            var json = BuildJsonFromByteArrays(
                () => ServiceData1, 
                () => ServiceData2, 
                () => ServiceData3,
                () => ServiceData4);

            Services = new Wrapper<Small>(json).Entities;
        }

        public override void PrepareBeforeSave()
        {
            var json = new Wrapper<Small>(Services).AsString();
            BuildByteArraysFromJson(json, 
                (val) => ServiceData1 = val,
                (val) => ServiceData2 = val,
                (val) => ServiceData3 = val,
                (val) => ServiceData4 = val);
        }

        public CompanyTableEntity()
        {
            
        }

        public CompanyTableEntity(PartionKeyRowKeyPair partitionKeyRowKeyPair, string name, IList<SmallUser> users)
        {
            PartitionKey = partitionKeyRowKeyPair.PartitionKey;
            RowKey = partitionKeyRowKeyPair.RowKey;

            Name = name;
            Users = users;
            TelemetryDataSinkSettings = new TelemetryDataSinkSettings();
        }
    }
}
