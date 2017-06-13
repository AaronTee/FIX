using FIX.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FIX.Core.Data;
using FIX.Data;

namespace FIX.Service
{
    public class UserBankAccountService : IUserBankAccountService
    {

        private IRepository<UserBankAccount> _userBankAccountRepo;

        public UserBankAccountService(IRepository<UserBankAccount> userBankAccountRepo)
        {
            _userBankAccountRepo = userBankAccountRepo;
        }

        public IQueryable<UserBankAccount> GetAsQueryable()
        {
            return _userBankAccountRepo.Table;
        }

        public UserBankAccount GetPrimaryAccount(User user)
        {
            return GetAsQueryable().Where(x => x.IsPrimary && x.UserId == user.UserId).FirstOrDefault();
        }
    }
}
