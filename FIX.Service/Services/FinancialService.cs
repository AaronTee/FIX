using FIX.Service.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIX.Service
{
    public class FinancialService : IFinancialService
    {
        private IUnitOfWork _uow;
        private IDocService _docService;

        public FinancialService(IUnitOfWork uow, IDocService docService)
        {
            _uow = uow;
            _docService = docService;
        }

        public UserWallet GetUserWallet(int userId)
        {
            return _uow.Repository<UserWallet>().GetAsQueryable(filter: x => x.UserId == userId).First();
        }

        public decimal? GetUserWalletBalance(Guid walletId)
        {
            return _uow.Repository<UserWallet>().GetAsQueryable(filter: x => x.WalletId == walletId).First()?.Balance;
        }

        public decimal? GetMatchingBonusReceivedAmount(Guid walletId)
        {
            var type = DBConstant.GetDescription(DBConstant.ETransactionType.Matching_Bonus);
            return _uow.Repository<WalletTransaction>().GetAsQueryable(filter: x => x.WalletId == walletId && x.TransactionType == type).Sum(x => x.Credit);
        }

        public decimal? GetReturnInterestReceivedAmount(Guid walletId)
        {
            var type = DBConstant.GetDescription(DBConstant.ETransactionType.Interest_Return);
            return _uow.Repository<WalletTransaction>().GetAsQueryable(filter: x => x.WalletId == walletId && x.TransactionType == type).Sum(x => x.Credit);
        }

        public decimal? GetWithdrawalAmount(Guid walletId)
        {
            var type = DBConstant.GetDescription(DBConstant.ETransactionType.Withdrawal);
            return _uow.Repository<WalletTransaction>().GetAsQueryable(filter: x => x.WalletId == walletId && x.TransactionType == type).Sum(x => x.Debit);
        }

        public IQueryable<WalletTransaction> GetWalletTransaction(Guid walletId)
        {
            var type = DBConstant.ETransactionType.Withdrawal;
            return _uow.Repository<WalletTransaction>().GetAsQueryable(filter: x => x.WalletId == walletId);
        }

        public bool hasSufficientBalance(decimal checkVal, Guid walletId)
        {
            return GetUserWalletBalance(walletId) >= checkVal;
        }

        public void SaveChange(int userId)
        {
            _uow.Save(userId);
        }

        public void TransactWalletCredit(DBConstant.EOperator optor, DBConstant.ETransactionType type, decimal amount, string docCode, Guid walletId)
        {
            var sType = Enum.GetName(type.GetType(), type);
            var userWallet = _uow.Repository<UserWallet>().GetByKey(walletId);
            var newWalletTransaction = new WalletTransaction();

            switch (optor)
            {
                case DBConstant.EOperator.ADD:
                    userWallet.Balance += amount;

                    newWalletTransaction = new WalletTransaction
                    {
                        WalletId = walletId,
                        ReferenceNo = docCode,
                        TransactionDate = DateTime.UtcNow,
                        Credit = amount,
                        TransactionType = sType
                    };
                    break;

                case DBConstant.EOperator.DEDUCT:
                    userWallet.Balance -= amount;

                    newWalletTransaction = new WalletTransaction
                    {
                        WalletId = walletId,
                        ReferenceNo = docCode,
                        TransactionDate = DateTime.UtcNow,
                        Credit = amount,
                        TransactionType = sType
                    };
                    break;

                default:
                    break;
            }

            _uow.Repository<WalletTransaction>().Insert(newWalletTransaction);
            _uow.Repository<UserWallet>().Update(userWallet);
        }
    }
}
