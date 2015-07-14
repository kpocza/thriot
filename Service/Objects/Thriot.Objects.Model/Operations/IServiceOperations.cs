namespace Thriot.Objects.Model.Operations
{
    public interface IServiceOperations
    {
        Service Get(string id);
    }

    public interface IPersistedServiceOperations : IServiceOperations { }
}
