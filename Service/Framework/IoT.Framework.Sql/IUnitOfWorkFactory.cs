namespace IoT.Framework.Sql
{
    public interface IUnitOfWorkFactory<out TUnitOfWork>
        where TUnitOfWork : IUnitOfWork
    {
        TUnitOfWork Create();
    }
}
