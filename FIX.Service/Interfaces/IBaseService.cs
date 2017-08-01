namespace FIX.Service
{
    public interface IBaseService
    {
        void EndTransaction(int userId, bool goAsync = false);
        void StartTransaction();
    }
}