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

        public FinancialService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public UserWallet GetUserWallet(int userId)
        {
            return _uow.Repository<UserWallet>().GetAsQueryable(filter: x => x.UserId == userId).First();
        }

        public decimal? GetUserWalletBalance(int userId)
        {
            return _uow.Repository<UserWallet>().GetAsQueryable(filter: x => x.UserId == userId).First()?.Balance;
        }

        public bool hasSufficientBalance(decimal checkVal, int userId)
        {
            return GetUserWalletBalance(userId) >= checkVal;
        }

        public void SaveChange(int userId)
        {
            _uow.Save(userId);
        }
    }
}
