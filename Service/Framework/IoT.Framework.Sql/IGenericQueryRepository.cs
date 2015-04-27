using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Thriot.Framework.Sql
{
    public interface IGenericQueryRepository<T> where T : class
    {
        void Create(T entity);

        void Delete(T entity);

        ICollection<T> List(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeExpressions);
    }
}
