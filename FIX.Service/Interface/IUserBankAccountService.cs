using FIX.Core.Data;
using System.Collections.Generic;
using System.Linq;

namespace FIX.Service.Interface
{
    public interface IUserBankAccountService : IBaseService
    {
        UserBankAccount GetPrimaryAccount(User user);
    }
}
