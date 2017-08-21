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
        decimal? GetUserWalletAuthorizedBalance(Guid walletId);
        decimal? GetUserWalletAvailableBalance(Guid walletId);
        IQueryable<WalletTransaction> GetWalletTransaction(Guid walletId);
        decimal? GetWithdrawalAmount(Guid walletId);
        void InsertWithdrawal(Withdrawal entity);
        bool hasSufficientBalance(decimal checkVal, Guid walletId);
        void SaveChange(int userId);
        Preauth GetPreauthTransaction(int? PreauthId);
        IQueryable<Preauth> GetPendingPreauthorizeTransactionList(Guid walletId);
        IQueryable<Preauth> GetAllWalletPendingPreauthorizeTransactionList(int? userId);
        void PreauthorizeWalletCredit(DBConstant.EOperator optor, DBConstant.ETransactionType type, decimal amount, string docCode, Guid walletId);
        void TransactWalletCredit(DBConstant.EOperator optor, DBConstant.ETransactionType type, decimal amount, string docCode, Guid walletId);
    }
}