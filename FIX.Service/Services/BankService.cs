using System.Linq;
using System.Data.Entity;
using System;
using FIX.Service.Entities;

namespace FIX.Service
{
    public class BankService : IBankService
    {
        private IRepository<Bank> _bankService;

        public BankService(IUnitOfWork uow)
        {
            _bankService = uow.Repository<Bank>();
        }

        public IQueryable<Bank> GetAllBank()
        {
            return _bankService.GetAsQueryable();
        }

        public Bank GetBank(UserBankAccount userBankAccount)
        {
            return _bankService.GetByKey(userBankAccount.BankId);
        }
    }
}