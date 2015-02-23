using System;
using System.Linq.Expressions;

namespace IoT.Framework.Sql
{
    public interface IRepository<T>: IGenericQueryRepository<T>
        where T : class, IEntity
    {
        T Get(string id, params Expression<Func<T, object>>[] includeExpressions);
    }
}
