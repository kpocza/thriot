using System.Collections.Generic;
using System.Linq;
using IoT.Framework;
using IoT.Framework.Exceptions;
using IoT.Management.Model;
using IoT.UnitTestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoT.Management.Operations.Tests
{
    [TestClass]
    public class ServiceOperationsTest
    {
        [TestMethod]
        public void CreateServiceTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var serviceOperations = environmentFactory.MgmtServiceOperations;

            var companyId = CreateCompany();

            var id = serviceOperations.Create(new Service() { Company = new Company() { Id = companyId }, Name = "new service", ApiKey = Crypto.GenerateSafeRandomToken() });

            Assert.AreEqual(32, id.Length);
        }

        [TestMethod]
        public void GetServiceTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var serviceOperations = environmentFactory.MgmtServiceOperations;

            var companyId = CreateCompany();

            var service1 = new Service() { Company = new Company() { Id = companyId }, Name = "new service1", ApiKey = Crypto.GenerateSafeRandomToken() };
            var service2 = new Service() { Company = new Company() { Id = companyId }, Name = "new service2", ApiKey = Crypto.GenerateSafeRandomToken() };

            var service1Id = serviceOperations.Create(service1);
            var service2Id = serviceOperations.Create(service2);

            var s1 = serviceOperations.Get(service1Id);
            var s2 = serviceOperations.Get(service2Id);

            Assert.AreEqual(service1Id, s1.Id);
            Assert.AreEqual(service1.Name, s1.Name);
            Assert.AreEqual(companyId, s1.Company.Id);
            Assert.AreEqual(32, s1.ApiKey.Length);
            Assert.AreEqual(service2Id, s2.Id);
            Assert.AreEqual(service2.Name, s2.Name);
            Assert.AreEqual(companyId, s2.Company.Id);
            Assert.AreEqual(32, s2.ApiKey.Length);
        }

        [TestMethod]
        public void ListServicesTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var companyOperations = environmentFactory.MgmtCompanyOperations;
            var serviceOperations = environmentFactory.MgmtServiceOperations;

            var companyId = CreateCompany();

            var service1 = new Service() { Company = new Company() { Id = companyId }, Name = "new service1", ApiKey = Crypto.GenerateSafeRandomToken() };
            var service2 = new Service() { Company = new Company() { Id = companyId }, Name = "new service2", ApiKey = Crypto.GenerateSafeRandomToken() };

            var service1Id = serviceOperations.Create(service1);
            var service2Id = serviceOperations.Create(service2);

            var services = companyOperations.ListServices(companyId);
            Assert.AreEqual(2, services.Count);

            var s1 = services.Single(s => s.Id == service1Id);
            var s2 = services.Single(s => s.Id == service2Id);

            Assert.AreEqual(service1Id, s1.Id);
            Assert.AreEqual(service1.Name, s1.Name);
            Assert.AreEqual(service2Id, s2.Id);
            Assert.AreEqual(service2.Name, s2.Name);
        }

        [TestMethod]
        public void DeleteServiceTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var companyOperations = environmentFactory.MgmtCompanyOperations;
            var serviceOperations = environmentFactory.MgmtServiceOperations;

            var companyId = CreateCompany();

            var service = new Service() { Company = new Company() { Id = companyId }, Name = "new service", ApiKey = Crypto.GenerateSafeRandomToken() };

            var id = serviceOperations.Create(service);

            serviceOperations.Delete(id);

            var services = companyOperations.ListServices(companyId);

            AssertionHelper.AssertThrows<NotFoundException>(() => serviceOperations.Get(id));

            Assert.AreEqual(0, services.Count);
        }

        [TestMethod]
        public void UpdateServiceTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var companyOperations = environmentFactory.MgmtCompanyOperations;
            var serviceOperations = environmentFactory.MgmtServiceOperations;

            var companyId = CreateCompany();

            var service = new Service() { Company = new Company() { Id = companyId }, Name = "new service", ApiKey = Crypto.GenerateSafeRandomToken() };

            var id = serviceOperations.Create(service);

            var newService = serviceOperations.Get(id);

            newService.Name += "mod";
            newService.ApiKey = Identity.Next();
            serviceOperations.Update(newService);

            var updatedService = serviceOperations.Get(id);
            var services = companyOperations.ListServices(companyId);

            Assert.AreEqual("new servicemod", updatedService.Name);
            Assert.AreEqual(newService.ApiKey, updatedService.ApiKey);
            Assert.AreEqual(1, services.Count);
            Assert.AreEqual("new servicemod", services[0].Name);
        }

        [TestMethod]
        public void UpdateService2Test()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var serviceOperations = environmentFactory.MgmtServiceOperations;

            var companyId = CreateCompany();

            var service = new Service() { Company = new Company() { Id = companyId }, Name = "new service", ApiKey = Crypto.GenerateSafeRandomToken() };
            var id = serviceOperations.Create(service);
            
            var newService = serviceOperations.Get(id);

            newService.TelemetryDataSinkSettings = new TelemetryDataSinkSettings()
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

            serviceOperations.Update(newService);

            var updatedService = serviceOperations.Get(id);

            Assert.AreEqual("new service", updatedService.Name);
            Assert.AreEqual(1, updatedService.TelemetryDataSinkSettings.Incoming.Count());
            Assert.AreEqual("test", updatedService.TelemetryDataSinkSettings.Incoming.First().SinkName);
            Assert.AreEqual(2, updatedService.TelemetryDataSinkSettings.Incoming.First().Parameters.Count);
        }

        private string CreateCompany()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var userOperations = environmentFactory.MgmtUserOperations;
            var companyOperations = environmentFactory.MgmtCompanyOperations;

            var salt = Crypto.GenerateSalt();
            var passwordHash = Crypto.CalcualteHash("password", salt);
            var userId = userOperations.Create(new User() { Name = "new user", Email = EmailHelper.Generate() }, passwordHash, salt);

            var company = new Company { Name = "new company" };

            var companyId = companyOperations.Create(company, userId);
            return companyId;
        }
    }
}

