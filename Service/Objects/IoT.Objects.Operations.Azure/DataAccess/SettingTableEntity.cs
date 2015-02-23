using IoT.Framework.Azure.DataAccess;

namespace IoT.Objects.Operations.Azure.DataAccess
{
    public class SettingTableEntity : PreparableTableEntity
    {
        public string Value { get; set; }
    }
}
