using FIX.Service.Entities;
using System;
using System.Linq;
using static FIX.Service.DBConstant;

namespace FIX.Service
{
    public interface IFinancialService
    {
        decimal? GetMatchingBonusReceivedAmount(Guid walletId);
        decimal? GetReturnInterestReceivedAmount(Guid walletId);
        UserWallet GetUserWallet(int userId);
        decimal? GetUserWalletBalance(Guid walletId);
        IQueryable<WalletTransaction> GetWalletTransaction(Guid walletId);
        decimal? GetWithdrawalAmount(Guid walletId);
        bool hasSufficientBalance(decimal checkVal, Guid walletId);
        void SaveChange(int userId);
        void TransactWalletCredit(DBConstant.EOperator optor, DBConstant.ETransactionType type, decimal amount, string docCode, Guid walletId);
    }
}