using Thriot.Objects.Model;
using Thriot.Objects.Model.Operations;

namespace Thriot.Objects.Common.CachingOperations
{
    public class SettingOperations : CachingBase<Setting>, ISettingOperations
    {
        private readonly IPersistedSettingOperations _settingOperations;

        protected override string Prefix
        {
            get { return "Setting"; }
        }

        public SettingOperations(IPersistedSettingOperations settingOperations)
        {
            _settingOperations = settingOperations;
        }

        public Setting Get(SettingId id)
        {
            return Get(id, i => _settingOperations.Get((SettingId)i));
        }
    }
}
