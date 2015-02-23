﻿using IoT.Framework.Azure.DataAccess;
using IoT.Framework.Azure.TableOperations;

namespace IoT.Management.Operations.Azure.DataAccess
{
    public class UserRepository : Repository<UserTableEntity>
    {
        public UserRepository(ITableEntityOperation tableEntityOperation)
            : base(tableEntityOperation)
        {
        }

        protected override string TableName
        {
            get { return "User"; }
        }
    }
}
