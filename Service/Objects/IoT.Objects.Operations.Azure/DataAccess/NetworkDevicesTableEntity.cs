using System.Collections.Generic;
using Thriot.Framework;
using Thriot.Framework.Azure.DataAccess;
using Thriot.Objects.Model;

namespace Thriot.Objects.Operations.Azure.DataAccess
{
    public class NetworkDevicesTableEntity : PreparableTableEntity
    {
        public string Name { get; set; }

        public IList<Small> Devices { get; set; }


        public byte[] DeviceData1 { get; set; }
        public byte[] DeviceData2 { get; set; }
        public byte[] DeviceData3 { get; set; }
        public byte[] DeviceData4 { get; set; }

        public override void PrepareAfterLoad()
        {
            var jsonDevice = BuildJsonFromByteArrays(
                () => DeviceData1,
                () => DeviceData2,
                () => DeviceData3,
                () => DeviceData4);

            Devices = new Wrapper<Small>(jsonDevice).Entities;
        }

        public override void PrepareBeforeSave()
        {
            var jsonDevice = new Wrapper<Small>(Devices).AsString();
            BuildByteArraysFromJson(jsonDevice,
                (val) => DeviceData1 = val,
                (val) => DeviceData2 = val,
                (val) => DeviceData3 = val,
                (val) => DeviceData4 = val);
        }
    }
}
