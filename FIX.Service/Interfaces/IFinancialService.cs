using FIX.Service.Entities;

namespace FIX.Service
{
    public interface IFinancialService
    {
        decimal? GetUserWalletBalance(int userId);
        bool hasSufficientBalance(decimal checkVal, int userId);
        UserWallet GetUserWallet(int userId);
        void SaveChange(int userId);
    }
}