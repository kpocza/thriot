namespace Thriot.Management.Model.Operations
{
    public interface ISettingOperations
    {
        void Create(Setting setting);

        Setting Get(SettingId id);

        void Update(Setting company);

        // Just for testing
        void Delete(SettingId id);
    }
}