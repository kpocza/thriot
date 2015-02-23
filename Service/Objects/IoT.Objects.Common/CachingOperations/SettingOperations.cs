using IoT.Objects.Model;
using IoT.Objects.Model.Operations;

namespace IoT.Objects.Common.CachingOperations
{
    public class SettingOperations : CachingBase<Setting>, ISettingOperations
    {
        private readonly ISettingOperations _settingOperations;

        protected override string Prefix
        {
            get { return "Setting"; }
        }

        public SettingOperations(ISettingOperations settingOperations)
        {
            _settingOperations = settingOperations;
        }

        public Setting Get(SettingId id)
        {
            return Get(id, i => _settingOperations.Get((SettingId)i));
        }
    }
}
