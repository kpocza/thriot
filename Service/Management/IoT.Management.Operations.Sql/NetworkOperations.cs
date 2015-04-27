using System.Collections.Generic;
using System.Linq;
using Thriot.Framework;
using Thriot.Framework.Exceptions;
using Thriot.Management.Model;
using Thriot.Management.Model.Operations;
using Thriot.Management.Operations.Sql.DataAccess;

namespace Thriot.Management.Operations.Sql
{
    public class NetworkOperations : INetworkOperations
    {
        private readonly IManagementUnitOfWorkFactory _managementUnitOfWorkFactory;

        public NetworkOperations(IManagementUnitOfWorkFactory managementUnitOfWorkFactory)
        {
            _managementUnitOfWorkFactory = managementUnitOfWorkFactory;
        }

        public string Create(Network network)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                var networkIdentity = Identity.NextIncremental();

                network.Id = networkIdentity;
                network.ChildNetworks = null;
                network.Company = unitOfWork.GetCompanyRepository().Get(network.Company.Id);
                network.Service = unitOfWork.GetServiceRepository().Get(network.Service.Id);
                network.ParentNetwork = network.ParentNetwork!= null ? unitOfWork.GetNetworkRepository().Get(network.ParentNetwork.Id) : null;
                network.TelemetryDataSinkSettings = new TelemetryDataSinkSettings();
                
                unitOfWork.GetNetworkRepository().Create(network);

                unitOfWork.Commit();

                return networkIdentity;
            }
        }

        public Network Get(string id)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                var network = unitOfWork.GetNetworkRepository()
                    .Get(id, n => n.Company, n => n.Service, n => n.ParentNetwork);

                if(network == null)
                    throw new NotFoundException();

                return network;
            }
        }

        public void Update(Network network)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                var networkEntity = unitOfWork.GetNetworkRepository().Get(network.Id);

                if (networkEntity == null)
                    throw new NotFoundException();

                networkEntity.Name = network.Name;
                networkEntity.TelemetryDataSinkSettings = network.TelemetryDataSinkSettings;
                
                unitOfWork.Commit();
            }
        }

        public void Delete(string id)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                var networkRepository = unitOfWork.GetNetworkRepository();

                var networkEntity = networkRepository.Get(id);

                if (networkEntity == null)
                    throw new NotFoundException();

                networkRepository.Delete(networkEntity);

                unitOfWork.Commit();
            }
        }

        public IList<Small> ListNetworks(string networkId)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                var network = unitOfWork.GetNetworkRepository().Get(networkId, n => n.ChildNetworks);
                if (network == null)
                    throw new NotFoundException();

                return network.ChildNetworks.Select(n => new Small { Id = n.Id, Name = n.Name }).ToList();
            }
        }

        public IList<Small> ListDevices(string networkId)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                var network = unitOfWork.GetNetworkRepository().Get(networkId, n => n.Devices);
                if (network == null)
                    throw new NotFoundException();

                return network.Devices.Select(d => new Small { Id = d.Id, Name = d.Name }).ToList();
            }
        }
    }
}
