﻿using IoT.Framework.Azure.DataAccess;
using Microsoft.WindowsAzure.Storage.Table;

namespace IoT.Management.Operations.Azure.DataAccess
{
    public class LoginUserTableEntity : PreparableTableEntity
    {
        public string Salt { get; set; }

        public string PasswordHash { get; set; }

        public string UserId { get; set; }

        public LoginUserTableEntity()
        {
            
        }

        public LoginUserTableEntity(string partitionKey, string email, string passwordHash, string salt, string userId)
        {
            PartitionKey = partitionKey;
            RowKey = email;

            Salt = salt;
            PasswordHash = passwordHash;
            UserId = userId;
        }
    }
}
