namespace FIX.Service
{
    public interface IUnitOfWork
    {
        FIXEntities Context { get; }

        void Dispose();
        IRepository<T> Repository<T>() where T : class;
        bool Save();
        bool Save(int userId, bool goAsync = false);
    }
}