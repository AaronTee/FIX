using System.Linq;
using System.Data.Entity;
using System;
using FIX.Service.Entities;

namespace FIX.Service
{
    public class UserBankAccountService : EntityService<UserBankAccount>, IUserBankAccountService
    {
        public UserBankAccountService(IUnitOfWork uow) : base(uow)
        {
        }

        public UserBankAccount GetUserBankAccountBy(UserProfile userProfile)
        {
            return Get(filter: x => x.UserId == userProfile.UserId).FirstOrDefault();
        }
    }
}