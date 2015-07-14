namespace Thriot.Objects.Model.Operations
{
    public interface ISettingOperations
    {
        Setting Get(SettingId id);
    }

    public interface IPersistedSettingOperations : ISettingOperations { }
}
