using System.Collections.Generic;
using IoT.Framework;
using IoT.Framework.Azure.DataAccess;
using IoT.Management.Model;

namespace IoT.Management.Operations.Azure.DataAccess
{
    public class UserTableEntity : PreparableTableEntity
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public IList<Small> Companies { get; set; }

        public bool Activated { get; set; }

        public string ActivationCode { get; set; }


        public string CompaniesStorage
        {
            get { return new Wrapper<Small>(Companies).AsString(); }
            set { Companies = new Wrapper<Small>(value).Entities; }
        }
        
        public UserTableEntity()
        {
            
        }

        public UserTableEntity(PartionKeyRowKeyPair partitionKeyRowKeyPair, string name, string email, IList<Small> companies, bool activated, string activationCode)
        {
            PartitionKey = partitionKeyRowKeyPair.PartitionKey;
            RowKey = partitionKeyRowKeyPair.RowKey;

            Name = name;
            Companies = companies;
            Email = email;
            Activated = activated;
            ActivationCode = activationCode;
        }
    }
}
