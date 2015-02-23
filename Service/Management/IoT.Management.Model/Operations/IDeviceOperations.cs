namespace IoT.Management.Model.Operations
{
    public interface IDeviceOperations
    {
        string Create(Device device);

        Device Get(string id);

        void Update(Device device);

        void Delete(string id);
    }
}
