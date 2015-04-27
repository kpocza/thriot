using System;
using System.Linq.Expressions;

namespace Thriot.Framework.Sql
{
    public interface IRepository<T>: IGenericQueryRepository<T>
        where T : class, IEntity
    {
        T Get(string id, params Expression<Func<T, object>>[] includeExpressions);
    }
}
