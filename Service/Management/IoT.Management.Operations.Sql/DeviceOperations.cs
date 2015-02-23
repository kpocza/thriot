using IoT.Framework;
using IoT.Framework.Exceptions;
using IoT.Management.Model;
using IoT.Management.Model.Operations;
using IoT.Management.Operations.Sql.DataAccess;

namespace IoT.Management.Operations.Sql
{
    public class DeviceOperations : IDeviceOperations
    {
        private readonly IManagementUnitOfWorkFactory _managementUnitOfWorkFactory;

        public DeviceOperations(IManagementUnitOfWorkFactory managementUnitOfWorkFactory)
        {
            _managementUnitOfWorkFactory = managementUnitOfWorkFactory;
        }

        public string Create(Device device)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                var deviceIdentity = Identity.NextIncremental();

                device.Id = deviceIdentity;
                device.Company = unitOfWork.GetCompanyRepository().Get(device.Company.Id);
                device.Service = unitOfWork.GetServiceRepository().Get(device.Service.Id);
                device.Network = unitOfWork.GetNetworkRepository().Get(device.Network.Id);
                
                unitOfWork.GetDeviceRepository().Create(device);

                unitOfWork.Commit();

                return deviceIdentity;
            }
        }

        public Device Get(string id)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                var device = unitOfWork.GetDeviceRepository().Get(id, d => d.Company, d => d.Service, d => d.Network);

                if(device == null)
                    throw new NotFoundException();

                return device;
            }
        }

        public void Update(Device device)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                var deviceEntity = unitOfWork.GetDeviceRepository().Get(device.Id);

                if (deviceEntity == null)
                    throw new NotFoundException();

                deviceEntity.Name = device.Name;
                deviceEntity.DeviceKey = device.DeviceKey;
                deviceEntity.NumericId = device.NumericId;

                unitOfWork.Commit();
            }
        }

        public void Delete(string id)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                var deviceRepository = unitOfWork.GetDeviceRepository();

                var deviceEntity = deviceRepository.Get(id);
                if (deviceEntity == null)
                    throw new NotFoundException();

                deviceRepository.Delete(deviceEntity);

                unitOfWork.Commit();
            }
        }
    }
}
