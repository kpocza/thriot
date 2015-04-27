using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Thriot.Framework.Sql
{
    public class GenericQueryRepository<T> : IGenericQueryRepository<T> where T : class
    {
        protected readonly DbContext DbContext;

        protected GenericQueryRepository(DbContext dbContext)
        {
            DbContext = dbContext;
        }

        public virtual void Create(T entity)
        {
            DbContext.Set<T>().Add(entity);
        }

        public virtual void Delete(T entity)
        {
            DbContext.Set<T>().Remove(entity);
        }

        public virtual ICollection<T> List(Expression<Func<T, bool>> filter,
            params Expression<Func<T, object>>[] includeExpressions)
        {
            IQueryable<T> query = DbContext.Set<T>();
            foreach (var includeExpression in includeExpressions)
            {
                query = query.Include(includeExpression);
            }
            return query.Where(filter).ToList();
        }
    }
}
