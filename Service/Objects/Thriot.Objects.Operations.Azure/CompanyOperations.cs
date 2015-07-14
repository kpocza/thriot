using Thriot.Framework.Azure.DataAccess;
using Thriot.Framework.Azure.TableOperations;
using Thriot.Framework.Exceptions;
using Thriot.Objects.Model;
using Thriot.Objects.Model.Operations;
using Thriot.Objects.Operations.Azure.DataAccess;

namespace Thriot.Objects.Operations.Azure
{
    public class CompanyOperations : IPersistedCompanyOperations
    {
        private readonly ITableEntityOperation _tableEntityOperation;

        public CompanyOperations(ICloudStorageClientFactory cloudStorageClientFactory)
        {
            _tableEntityOperation = cloudStorageClientFactory.GetTableEntityOperation();
        }

        public Company Get(string id)
        {
            var companyKey = PartionKeyRowKeyPair.CreateFromIdentity(id);

            var companyRepository = new CompanyRepository(_tableEntityOperation);

            var companyTableEntity = companyRepository.Get(companyKey);

            if (companyTableEntity == null)
                throw new NotFoundException();

            return new Company
            {
                Id = id,
                TelemetryDataSinkSettings = companyTableEntity.TelemetryDataSinkSettings
            };
        }
    }
}