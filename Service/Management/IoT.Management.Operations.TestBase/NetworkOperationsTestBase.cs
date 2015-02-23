using System.Collections.Generic;
using System.Linq;
using IoT.Framework;
using IoT.Framework.Exceptions;
using IoT.Management.Model;
using IoT.Management.Model.Operations;
using IoT.UnitTestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoT.Management.Operations.TestBase
{
    public abstract class NetworkOperationsTestBase
    {
        #region Under service

        public virtual void CreateNetworkUnderServiceTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var networkOperations = environmentFactory.MgmtNetworkOperations;

            var companyServiceIdPair = CreateCompanyAndService();

            var id =
                networkOperations.Create(new Network()
                {
                    Service = new Service() { Id = companyServiceIdPair.ServiceId },
                    Company = new Company() { Id = companyServiceIdPair.CompanyId },
                    Name = "new network",
                    NetworkKey = Crypto.GenerateSafeRandomToken()
                });

            Assert.AreEqual(32, id.Length);
        }

        public virtual void GetNetworkUnderServiceTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var networkOperations = environmentFactory.MgmtNetworkOperations;

            var companyServiceIdPair = CreateCompanyAndService();

            var network1 = new Network()
            {
                Service = new Service() { Id = companyServiceIdPair.ServiceId },
                Company = new Company() { Id = companyServiceIdPair.CompanyId },
                Name = "new network1",
                NetworkKey = Crypto.GenerateSafeRandomToken()
            };
            var network2 = new Network()
            {
                Service = new Service() { Id = companyServiceIdPair.ServiceId },
                Company = new Company() { Id = companyServiceIdPair.CompanyId },
                Name = "new network2",
                NetworkKey = Crypto.GenerateSafeRandomToken()
            };

            var network1Id = networkOperations.Create(network1);
            var network2Id = networkOperations.Create(network2);

            var dg1 = networkOperations.Get(network1Id);
            var dg2 = networkOperations.Get(network2Id);

            Assert.AreEqual(network1Id, dg1.Id);
            Assert.AreEqual(network1.Name, dg1.Name);
            Assert.AreEqual(companyServiceIdPair.CompanyId, dg1.Company.Id);
            Assert.AreEqual(companyServiceIdPair.ServiceId, dg1.Service.Id);
            Assert.AreEqual(network1.NetworkKey, dg1.NetworkKey);
            Assert.IsNull(dg1.ParentNetwork);
            Assert.AreEqual(network2Id, dg2.Id);
            Assert.AreEqual(network2.Name, dg2.Name);
            Assert.AreEqual(companyServiceIdPair.CompanyId, dg2.Company.Id);
            Assert.AreEqual(companyServiceIdPair.ServiceId, dg2.Service.Id);
            Assert.AreEqual(network2.NetworkKey, dg2.NetworkKey);
            Assert.IsNull(dg2.ParentNetwork);
        }

        public virtual void ListNetworkUnderServiceTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var serviceOperations = environmentFactory.MgmtServiceOperations;
            var networkOperations = environmentFactory.MgmtNetworkOperations;

            var companyServiceIdPair = CreateCompanyAndService();

            var network1 = new Network()
            {
                Service = new Service() { Id = companyServiceIdPair.ServiceId },
                Company = new Company() { Id = companyServiceIdPair.CompanyId },
                Name = "new network1",
                NetworkKey = Crypto.GenerateSafeRandomToken()
            };
            var network2 = new Network()
            {
                Service = new Service() { Id = companyServiceIdPair.ServiceId },
                Company = new Company() { Id = companyServiceIdPair.CompanyId },
                Name = "new network2",
                NetworkKey = Crypto.GenerateSafeRandomToken()
            };

            var network1Id = networkOperations.Create(network1);
            var network2Id = networkOperations.Create(network2);

            var networks = serviceOperations.ListNetworks(companyServiceIdPair.ServiceId);

            Assert.AreEqual(2, networks.Count);

            var dg1 = networks.Single(d => d.Id == network1Id);
            var dg2 = networks.Single(d => d.Id == network2Id);

            Assert.AreEqual(network1Id, dg1.Id);
            Assert.AreEqual(network1.Name, dg1.Name);
            Assert.AreEqual(network2Id, dg2.Id);
            Assert.AreEqual(network2.Name, dg2.Name);
        }

        public virtual void DeleteNetworkUnderServiceTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var serviceOperations = environmentFactory.MgmtServiceOperations;
            var networkOperations = environmentFactory.MgmtNetworkOperations;

            var companyServiceIdPair = CreateCompanyAndService();

            var id =
                networkOperations.Create(new Network()
                {
                    Service = new Service() { Id = companyServiceIdPair.ServiceId },
                    Company = new Company() { Id = companyServiceIdPair.CompanyId },
                    Name = "new network",
                    NetworkKey = Crypto.GenerateSafeRandomToken()
                });

            networkOperations.Delete(id);

            var networks = serviceOperations.ListNetworks(companyServiceIdPair.ServiceId);

            AssertionHelper.AssertThrows<NotFoundException>(() => networkOperations.Get(id));

            Assert.AreEqual(0, networks.Count);
        }

        public virtual void UpdateNetworkUnderServiceTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var serviceOperations = environmentFactory.MgmtServiceOperations;
            var networkOperations = environmentFactory.MgmtNetworkOperations;

            var companyServiceIdPair = CreateCompanyAndService();

            var id =
                networkOperations.Create(new Network()
                {
                    Service = new Service() { Id = companyServiceIdPair.ServiceId },
                    Company = new Company() { Id = companyServiceIdPair.CompanyId },
                    Name = "new network",
                    NetworkKey = Crypto.GenerateSafeRandomToken()
                });

            var newnetwork = networkOperations.Get(id);

            newnetwork.Name += "mod";
            networkOperations.Update(newnetwork);

            var updatednetwork = networkOperations.Get(id);
            var networks = serviceOperations.ListNetworks(companyServiceIdPair.ServiceId);

            Assert.AreEqual("new networkmod", updatednetwork.Name);
            Assert.AreEqual(1, networks.Count);
            Assert.AreEqual("new networkmod", networks[0].Name);
            Assert.AreEqual(newnetwork.NetworkKey, updatednetwork.NetworkKey);
        }

        public virtual void UpdateNetworkUnderService2Test()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var networkOperations = environmentFactory.MgmtNetworkOperations;

            var companyServiceIdPair = CreateCompanyAndService();

            var id =
                networkOperations.Create(new Network()
                {
                    Service = new Service() { Id = companyServiceIdPair.ServiceId },
                    Company = new Company() { Id = companyServiceIdPair.CompanyId },
                    Name = "new network",
                    NetworkKey = Crypto.GenerateSafeRandomToken()
                });

            var newnetwork = networkOperations.Get(id);
            newnetwork.TelemetryDataSinkSettings = new TelemetryDataSinkSettings()
            {
                Incoming =
                    new List<TelemetryDataSinkParameters>
                    {
                        new TelemetryDataSinkParameters
                        {
                            SinkName = "test",
                            Parameters = new Dictionary<string, string> {{"k1", "v1"}, {"k2", "v2"}}
                        }
                    }
            };
            networkOperations.Update(newnetwork);

            var updatednetwork = networkOperations.Get(id);

            Assert.AreEqual("new network", updatednetwork.Name);
            Assert.AreEqual(1, updatednetwork.TelemetryDataSinkSettings.Incoming.Count());
            Assert.AreEqual("test", updatednetwork.TelemetryDataSinkSettings.Incoming.First().SinkName);
            Assert.AreEqual(2, updatednetwork.TelemetryDataSinkSettings.Incoming.First().Parameters.Count);
        }

        #endregion

        #region Under other network

        public virtual void CreateNetworkUnderNetworkTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var networkOperations = environmentFactory.MgmtNetworkOperations;

            var companyServiceIdPair = CreateCompanyAndService();

            var parentNetworkId = CreateParentNetwork(networkOperations, companyServiceIdPair);

            var id = networkOperations.Create(new Network()
            {
                Name = "child network",
                ParentNetwork = new Network() { Id = parentNetworkId },
                Service = new Service() {Id = companyServiceIdPair.ServiceId},
                Company = new Company() { Id = companyServiceIdPair.CompanyId },
                NetworkKey = Crypto.GenerateSafeRandomToken()
            });

            Assert.AreEqual(32, id.Length);
        }

        public virtual void GetNetworkUnderNetworkTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var networkOperations = environmentFactory.MgmtNetworkOperations;

            var companyServiceIdPair = CreateCompanyAndService();

            var parentNetworkId = CreateParentNetwork(networkOperations, companyServiceIdPair);

            var network1 = new Network()
            {
                ParentNetwork = new Network() { Id = parentNetworkId },
                Service = new Service() { Id = companyServiceIdPair.ServiceId },
                Company = new Company() { Id = companyServiceIdPair.CompanyId },
                Name = "new network1",
                NetworkKey = Crypto.GenerateSafeRandomToken()
            };
            var network2 = new Network()
            {
                ParentNetwork = new Network() { Id = parentNetworkId },
                Service = new Service() { Id = companyServiceIdPair.ServiceId },
                Company = new Company() { Id = companyServiceIdPair.CompanyId },
                Name = "new network2",
                NetworkKey = Crypto.GenerateSafeRandomToken()
            };

            var network1Id = networkOperations.Create(network1);
            var network2Id = networkOperations.Create(network2);

            var dg1 = networkOperations.Get(network1Id);
            var dg2 = networkOperations.Get(network2Id);

            Assert.AreEqual(network1Id, dg1.Id);
            Assert.AreEqual(network1.Name, dg1.Name);
            Assert.AreEqual(companyServiceIdPair.CompanyId, dg1.Company.Id);
            Assert.AreEqual(companyServiceIdPair.ServiceId, dg1.Service.Id);
            Assert.AreEqual(parentNetworkId, dg1.ParentNetwork.Id);
            Assert.AreEqual(network2Id, dg2.Id);
            Assert.AreEqual(network2.Name, dg2.Name);
            Assert.AreEqual(companyServiceIdPair.CompanyId, dg2.Company.Id);
            Assert.AreEqual(companyServiceIdPair.ServiceId, dg2.Service.Id);
            Assert.AreEqual(parentNetworkId, dg2.ParentNetwork.Id);
        }

        public virtual void ListNetworkUnderNetworkTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var serviceOperations = environmentFactory.MgmtServiceOperations;
            var networkOperations = environmentFactory.MgmtNetworkOperations;

            var companyServiceIdPair = CreateCompanyAndService();

            var parentNetworkId = CreateParentNetwork(networkOperations, companyServiceIdPair);

            var network1 = new Network()
            {
                ParentNetwork = new Network() { Id = parentNetworkId },
                Service = new Service() { Id = companyServiceIdPair.ServiceId },
                Company = new Company() { Id = companyServiceIdPair.CompanyId },
                Name = "new network1",
                NetworkKey = Crypto.GenerateSafeRandomToken()
            };
            var network2 = new Network()
            {
                ParentNetwork = new Network() { Id = parentNetworkId },
                Service = new Service() { Id = companyServiceIdPair.ServiceId },
                Company = new Company() { Id = companyServiceIdPair.CompanyId },
                Name = "new network2",
                NetworkKey = Crypto.GenerateSafeRandomToken()
            };

            var network1Id = networkOperations.Create(network1);
            var network2Id = networkOperations.Create(network2);

            var networksS = serviceOperations.ListNetworks(companyServiceIdPair.ServiceId);

            Assert.AreEqual(1, networksS.Count);
            Assert.AreEqual(parentNetworkId, networksS[0].Id);

            var networksDg = networkOperations.ListNetworks(parentNetworkId);

            Assert.AreEqual(2, networksDg.Count);

            var dg1 = networksDg.Single(d => d.Id == network1Id);
            var dg2 = networksDg.Single(d => d.Id == network2Id);

            Assert.AreEqual(network1Id, dg1.Id);
            Assert.AreEqual(network1.Name, dg1.Name);
            Assert.AreEqual(network2Id, dg2.Id);
            Assert.AreEqual(network2.Name, dg2.Name);
        }

        public virtual void DeleteNetworkUnderNetworkTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var networkOperations = environmentFactory.MgmtNetworkOperations;

            var companyServiceIdPair = CreateCompanyAndService();

            var parentNetworkId = CreateParentNetwork(networkOperations, companyServiceIdPair);

            var id =
                networkOperations.Create(new Network()
                {
                    ParentNetwork = new Network() {Id = parentNetworkId},
                    Service = new Service() { Id = companyServiceIdPair.ServiceId },
                    Company = new Company() { Id = companyServiceIdPair.CompanyId },
                    Name = "new network",
                    NetworkKey = Crypto.GenerateSafeRandomToken()
                });

            networkOperations.Delete(id);

            var networks = networkOperations.ListNetworks(parentNetworkId);

            AssertionHelper.AssertThrows<NotFoundException>(() => networkOperations.Get(id));

            Assert.AreEqual(0, networks.Count);
        }

        public virtual void UpdateNetworkUnderNetworkTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var networkOperations = environmentFactory.MgmtNetworkOperations;

            var companyServiceIdPair = CreateCompanyAndService();

            var parentNetworkId = CreateParentNetwork(networkOperations, companyServiceIdPair);

            var id =
                networkOperations.Create(new Network()
                {
                    ParentNetwork = new Network() { Id = parentNetworkId },
                    Service = new Service() { Id = companyServiceIdPair.ServiceId },
                    Company = new Company() { Id = companyServiceIdPair.CompanyId },
                    Name = "new network",
                    NetworkKey = Crypto.GenerateSafeRandomToken()
                });

            var newnetwork = networkOperations.Get(id);

            newnetwork.Name += "mod";
            networkOperations.Update(newnetwork);

            var updatednetwork = networkOperations.Get(id);
            var networks = networkOperations.ListNetworks(parentNetworkId);

            Assert.AreEqual("new networkmod", updatednetwork.Name);
            Assert.AreEqual(1, networks.Count);
            Assert.AreEqual("new networkmod", networks[0].Name);
        }

        public virtual void UpdateNetworkUnderNetwork2Test()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var networkOperations = environmentFactory.MgmtNetworkOperations;

            var companyServiceIdPair = CreateCompanyAndService();

            var parentNetworkId = CreateParentNetwork(networkOperations, companyServiceIdPair);

            var id =
                networkOperations.Create(new Network()
                {
                    ParentNetwork = new Network() { Id = parentNetworkId },
                    Service = new Service() { Id = companyServiceIdPair.ServiceId },
                    Company = new Company() { Id = companyServiceIdPair.CompanyId },
                    Name = "new network",
                    NetworkKey = Crypto.GenerateSafeRandomToken()
                });

            var newnetwork = networkOperations.Get(id);
            newnetwork.TelemetryDataSinkSettings = new TelemetryDataSinkSettings()
            {
                Incoming =
                    new List<TelemetryDataSinkParameters>
                    {
                        new TelemetryDataSinkParameters
                        {
                            SinkName = "test",
                            Parameters = new Dictionary<string, string> {{"k1", "v1"}, {"k2", "v2"}}
                        }
                    }
            };
            networkOperations.Update(newnetwork);

            var updatednetwork = networkOperations.Get(id);

            Assert.AreEqual("new network", updatednetwork.Name);
            Assert.AreEqual(1, updatednetwork.TelemetryDataSinkSettings.Incoming.Count());
            Assert.AreEqual("test", updatednetwork.TelemetryDataSinkSettings.Incoming.First().SinkName);
            Assert.AreEqual(2, updatednetwork.TelemetryDataSinkSettings.Incoming.First().Parameters.Count);
        }
        
        #endregion

        private CompanyServiceIdPair CreateCompanyAndService()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var userOperations = environmentFactory.MgmtUserOperations;
            var companyOperations = environmentFactory.MgmtCompanyOperations;
            var serviceOperations = environmentFactory.MgmtServiceOperations;

            var salt = Crypto.GenerateSalt();
            var passwordHash = Crypto.CalcualteHash("password", salt);
            var userId = userOperations.Create(new User() { Name = "new user", Email = EmailHelper.Generate() }, passwordHash, salt);

            var company = new Company { Name = "new company" };
            var companyId = companyOperations.Create(company, userId);

            var service = new Service { Company = new Company { Id = companyId }, Name = "new service", ApiKey = Crypto.GenerateSafeRandomToken() };
            var serviceId = serviceOperations.Create(service);

            return new CompanyServiceIdPair { CompanyId = companyId, ServiceId = serviceId };
        }

        private string CreateParentNetwork(INetworkOperations networkOperations,
            CompanyServiceIdPair companyServiceIdPair)
        {
            var id =
                networkOperations.Create(new Network()
                {
                    Service = new Service() { Id = companyServiceIdPair.ServiceId },
                    Company = new Company() { Id = companyServiceIdPair.CompanyId },
                    Name = "new network",
                    NetworkKey = Crypto.GenerateSafeRandomToken()
                });
            return id;
        }

        private class CompanyServiceIdPair
        {
            internal string ServiceId;
            internal string CompanyId;
        }
    }
}

