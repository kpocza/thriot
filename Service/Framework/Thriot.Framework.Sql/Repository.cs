using System;
using Microsoft.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Thriot.Framework.Sql
{
    public abstract class Repository<T> : GenericQueryRepository<T>, IRepository<T> where T: class, IEntity
    {
        protected Repository(DbContext dbContext) : base(dbContext)
        {
        }

        public virtual T Get(string id, params Expression<Func<T, object>>[] includeExpressions)
        {
            IQueryable<T> query = DbContext.Set<T>();
            foreach (var includeExpression in includeExpressions)
            {
                query = query.Include(includeExpression);
            }
            return query.Where(entity => entity.Id == id).ToList().SingleOrDefault();
        }
    }
}
