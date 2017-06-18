using FIX.Service.Entities;

namespace FIX.Service
{
    public interface IUserBankAccountService : IEntityService<UserBankAccount>
    {
        UserBankAccount GetUserBankAccountBy(UserProfile userProfile);
    }
}