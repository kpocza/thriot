﻿using System.Data.Entity;
using System.Linq;
using IoT.Management.Model;

namespace IoT.Management.Operations.Sql.DataAccess
{
    public class LoginUserRepository
    {
        private readonly DbContext _dbContext;

        public LoginUserRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public LoginUser GetByEmail(string email)
        {
            return _dbContext.Set<LoginUser>().SingleOrDefault(lu => lu.Email == email);
        }

        public void Create(LoginUser loginUser)
        {
            _dbContext.Set<LoginUser>().Add(loginUser);
        }
    }
}
