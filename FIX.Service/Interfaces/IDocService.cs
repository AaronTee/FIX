namespace FIX.Service
{
    public interface IDocService
    {
        void SaveChange(int userId);
        string GetNextDocumentNumber(DBConstant.DBCDocSequence.EDocSequenceId docId);
    }
}