using System.Linq;
using FIX.Service.Entities;

namespace FIX.Service
{
    public interface IBankService
    {
        IQueryable<Bank> GetAllBank();
        Bank GetBank(UserBankAccount userBankAccount);
    }
}