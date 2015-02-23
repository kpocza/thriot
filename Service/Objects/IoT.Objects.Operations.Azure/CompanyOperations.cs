using IoT.Framework.Azure.DataAccess;
using IoT.Framework.Azure.TableOperations;
using IoT.Framework.Exceptions;
using IoT.Objects.Model;
using IoT.Objects.Model.Operations;
using IoT.Objects.Operations.Azure.DataAccess;

namespace IoT.Objects.Operations.Azure
{
    public class CompanyOperations : ICompanyOperations
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