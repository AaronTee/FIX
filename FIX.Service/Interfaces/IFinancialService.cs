using FIX.Service.Entities;
using System;
using static FIX.Service.DBConstant;

namespace FIX.Service
{
    public interface IFinancialService
    {
        decimal? GetUserWalletBalance(int userId);
        bool hasSufficientBalance(decimal checkVal, int userId);
        UserWallet GetUserWallet(int userId);
        void TransactWalletCredit(DBConstant.EOperator optor, DBConstant.ETransactionType type, decimal amount, string docCode, Guid walletId);
        void SaveChange(int userId);
    }
}