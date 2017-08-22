using FIX.Service.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FIX.Service.DBConstant;

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

        public decimal? GetUserWalletAuthorizedBalance(Guid walletId)
        {
            var wallet = _uow.Repository<UserWallet>().GetAsQueryable(filter: x => x.WalletId == walletId).First();
            return wallet?.AuthorizedBalance;
        }

        public decimal? GetUserWalletAvailableBalance(Guid walletId)
        {
            var wallet = _uow.Repository<UserWallet>().GetAsQueryable(filter: x => x.WalletId == walletId).First();
            return wallet?.Balance;
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
            return GetUserWalletAvailableBalance(walletId) >= checkVal;
        }

        public void SaveChange(int userId)
        {
            _uow.Save(userId);
        }

        public void InsertWithdrawal(Withdrawal entity)
        {
            _uow.Repository<Withdrawal>().Insert(entity);
        }

        public Preauth GetPreauthTransaction(int? PreauthId)
        {
            return _uow.Repository<Preauth>().GetByKey(PreauthId);
        }

        public IQueryable<Preauth> GetPendingPreauthorizeTransactionList(Guid walletId)
        {
            return _uow.Repository<Preauth>().GetAsQueryable(x => x.WalletId == walletId && x.StatusId == (int)EStatus.Pending);
        }

        /// <summary>
        /// Return all wallet transaction list (If multiple wallet is supported please do not use call this, use GetPendingPreauthorizeTransactionList instead).
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IQueryable<Preauth> GetAllWalletPendingPreauthorizeTransactionList(int? userId)
        {
            return _uow.Repository<Preauth>().GetAsQueryable(x => x.UserWallet.UserId == userId && x.StatusId == (int)EStatus.Pending);
        }

        public void PreauthorizeWalletCredit(DBConstant.EOperator optor, DBConstant.ETransactionType type, decimal amount, string docCode, Guid walletId)
        {
            var sType = Enum.GetName(type.GetType(), type);
            var userWallet = _uow.Repository<UserWallet>().GetByKey(walletId);

            Preauth newAuth = new Preauth
            {
                WalletId = userWallet.WalletId,
                ReferenceNo = docCode,
                TransactionDate = DateTime.UtcNow,
                TransactionType = sType,
                StatusId = (int)EStatus.Pending
            };

            switch (optor)
            {
                case DBConstant.EOperator.ADD:
                    newAuth.Credit = amount;
                    break;

                case DBConstant.EOperator.DEDUCT:
                    newAuth.Debit = amount;
                    userWallet.AuthorizedBalance += amount;
                    userWallet.Balance -= amount;
                    break;

                default:
                    break;
            }

            _uow.Repository<Preauth>().Insert(newAuth);
            _uow.Repository<UserWallet>().Update(userWallet);
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
                    userWallet.AuthorizedBalance -= amount;
                    newWalletTransaction = new WalletTransaction
                    {
                        WalletId = walletId,
                        ReferenceNo = docCode,
                        TransactionDate = DateTime.UtcNow,
                        Debit = amount,
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
