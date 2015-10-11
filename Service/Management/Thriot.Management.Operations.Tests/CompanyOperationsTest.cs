using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thriot.Framework;
using Thriot.Framework.Exceptions;
using Thriot.Management.Model;
using Thriot.TestHelpers;

namespace Thriot.Management.Operations.Tests
{
    [TestClass]
    public class CompanyOperationsTest
    {
        [TestMethod]
        public void CreateCompanyTest()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create(); 
            var userOperations = environmentFactory.ManagementEnvironment.MgmtUserOperations;
            var companyOperations = environmentFactory.ManagementEnvironment.MgmtCompanyOperations;

            var salt = Crypto.GenerateSalt();
            var passwordHash = Crypto.CalcualteHash("password", salt);
            var userId = userOperations.Create(new User() { Name = "new user", Email = EmailHelper.Generate() }, passwordHash, salt);

            var company = new Company { Name = "new company" };

            var companyId = companyOperations.Create(company, userId);

            Assert.AreEqual(32, companyId.Length);
        }

        [TestMethod]
        public void GetCompanyTest()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();
            var userOperations = environmentFactory.ManagementEnvironment.MgmtUserOperations;
            var companyOperations = environmentFactory.ManagementEnvironment.MgmtCompanyOperations;

            var salt = Crypto.GenerateSalt();
            var passwordHash = Crypto.CalcualteHash("password", salt);
            var userId = userOperations.Create(new User() { Name = "new user", Email = EmailHelper.Generate() }, passwordHash, salt);

            var company1 = new Company { Name = "new company1" };
            var company2 = new Company { Name = "new company2" };

            var company1Id = companyOperations.Create(company1, userId);
            var company2Id = companyOperations.Create(company2, userId);

            var c1 = companyOperations.Get(company1Id);
            var c2 = companyOperations.Get(company2Id);

            Assert.AreEqual(company1Id, c1.Id);
            Assert.AreEqual(company1.Name, c1.Name);
            Assert.AreEqual(company2Id, c2.Id);
            Assert.AreEqual(company2.Name, c2.Name);
        }

        [TestMethod]
        public void ListCompaniesTest()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();
            var userOperations = environmentFactory.ManagementEnvironment.MgmtUserOperations;
            var companyOperations = environmentFactory.ManagementEnvironment.MgmtCompanyOperations;

            var salt = Crypto.GenerateSalt();
            var passwordHash = Crypto.CalcualteHash("password", salt);
            var userId = userOperations.Create(new User() { Name = "new user", Email = EmailHelper.Generate() }, passwordHash, salt);

            var company1 = new Company { Name = "new company1" };
            var company2 = new Company { Name = "new company2" };

            var company1Id = companyOperations.Create(company1, userId);
            var company2Id = companyOperations.Create(company2, userId);

            var companies = userOperations.ListCompanies(userId);
            Assert.AreEqual(2, companies.Count);

            var c1 = companies.Single(c => c.Id == company1Id);
            var c2 = companies.Single(c => c.Id == company2Id);

            Assert.AreEqual(company1Id, c1.Id);
            Assert.AreEqual(company1.Name, c1.Name);
            Assert.AreEqual(company2Id, c2.Id);
            Assert.AreEqual(company2.Name, c2.Name);
        }

        [TestMethod]
        public void DeleteCompanyTest()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();
            var userOperations = environmentFactory.ManagementEnvironment.MgmtUserOperations;
            var companyOperations = environmentFactory.ManagementEnvironment.MgmtCompanyOperations;

            var salt = Crypto.GenerateSalt();
            var passwordHash = Crypto.CalcualteHash("password", salt);
            var userId = userOperations.Create(new User() { Name = "new user", Email = EmailHelper.Generate() }, passwordHash, salt);

            var company = new Company { Name = "new company" };

            var companyId = companyOperations.Create(company, userId);

            companyOperations.Delete(companyId);

            var companies = userOperations.ListCompanies(userId);

            AssertionHelper.AssertThrows<NotFoundException>(() => companyOperations.Get(companyId));
            AssertionHelper.AssertThrows<NotFoundException>(() => companyOperations.ListUsers(companyId));

            Assert.AreEqual(0, companies.Count);
        }

        [TestMethod]
        public void UpdateCompanyTest()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();
            var userOperations = environmentFactory.ManagementEnvironment.MgmtUserOperations;
            var companyOperations = environmentFactory.ManagementEnvironment.MgmtCompanyOperations;

            var salt = Crypto.GenerateSalt();
            var passwordHash = Crypto.CalcualteHash("password", salt);
            var email = EmailHelper.Generate();
            var userId = userOperations.Create(new User() { Name = "new user", Email = email }, passwordHash, salt);

            var company = new Company { Name = "new company" };

            var companyId = companyOperations.Create(company, userId);

            var newCompany = companyOperations.Get(companyId);

            newCompany.Name += "mod";
            companyOperations.Update(newCompany);

            var updatedCompany = companyOperations.Get(companyId);
            var companies = userOperations.ListCompanies(userId);
            var users = companyOperations.ListUsers(companyId);

            Assert.AreEqual("new companymod", updatedCompany.Name);
            Assert.AreEqual(1, users.Count);
            Assert.AreEqual("new user", users[0].Name);
            Assert.AreEqual(email, users[0].Email);
            Assert.AreEqual(1, companies.Count);
            Assert.AreEqual("new companymod", companies[0].Name);
        }

        [TestMethod]
        public void UpdateCompany2Test()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();
            var userOperations = environmentFactory.ManagementEnvironment.MgmtUserOperations;
            var companyOperations = environmentFactory.ManagementEnvironment.MgmtCompanyOperations;

            var salt = Crypto.GenerateSalt();
            var passwordHash = Crypto.CalcualteHash("password", salt);
            var userId = userOperations.Create(new User() { Name = "new user", Email = EmailHelper.Generate() }, passwordHash, salt);

            var company = new Company { Name = "new company" };

            var companyId = companyOperations.Create(company, userId);

            var newCompany = companyOperations.Get(companyId);

            newCompany.TelemetryDataSinkSettings = new TelemetryDataSinkSettings()
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

            companyOperations.Update(newCompany);

            var updatedCompany = companyOperations.Get(companyId);
            var users = companyOperations.ListUsers(companyId);

            Assert.AreEqual("new company", updatedCompany.Name);
            Assert.AreEqual(1, updatedCompany.TelemetryDataSinkSettings.Incoming.Count());
            Assert.AreEqual("test", updatedCompany.TelemetryDataSinkSettings.Incoming.First().SinkName);
            Assert.AreEqual(2, updatedCompany.TelemetryDataSinkSettings.Incoming.First().Parameters.Count);
            Assert.AreEqual(1, users.Count);
        }

        public void AddUserTest()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();
            var userOperations = environmentFactory.ManagementEnvironment.MgmtUserOperations;
            var companyOperations = environmentFactory.ManagementEnvironment.MgmtCompanyOperations;

            var salt = Crypto.GenerateSalt();
            var passwordHash = Crypto.CalcualteHash("password", salt);
            var userId = userOperations.Create(new User() { Name = "new user", Email = EmailHelper.Generate() }, passwordHash, salt);

            var company = new Company { Name = "new company" };

            var companyId = companyOperations.Create(company, userId);

            var user2Id = userOperations.Create(new User() { Name = "new user", Email = EmailHelper.Generate() }, passwordHash, salt);

            companyOperations.AddUser(companyId, user2Id);

            var users = companyOperations.ListUsers(companyId);
            Assert.AreEqual(2, users.Count);

            companyOperations.AddUser(companyId, user2Id);
            
            users = companyOperations.ListUsers(companyId);
            Assert.AreEqual(2, users.Count);
        }
    }
}

