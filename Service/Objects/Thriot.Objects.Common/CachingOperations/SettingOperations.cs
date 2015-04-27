using Thriot.Objects.Model;
using Thriot.Objects.Model.Operations;

namespace Thriot.Objects.Common.CachingOperations
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
