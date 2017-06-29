using System.Linq;
using System.Data.Entity;
using System;
using FIX.Service.Entities;
using System.Threading.Tasks;
using static FIX.Service.DBConstant;

namespace FIX.Service
{
    public class InvestmentService : IInvestmentService
    {

        private IUnitOfWork _uow;

        public InvestmentService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public IQueryable<Package> GetAllPackage()
        {
            return _uow.Repository<Package>().GetAsQueryable();
        }

        public IQueryable<UserPackage> GetAllUserPackage(int userId)
        {
            return _uow.Repository<UserPackage>().GetAsQueryable(filter: x=>x.UserId == userId);
        }

        public IQueryable<UserPackageDetail> GetAllUserPackageDetail(int userPackageId)
        {
            return _uow.Repository<UserPackageDetail>().GetAsQueryable(filter: x => x.UserPackageId == userPackageId);
        }

        public decimal GetPackageRate(int PackageId)
        {
            return _uow.Repository<Package>().GetByKey(PackageId).Rate;
        }

        public Package GetEntitledPackage(decimal amount)
        {
            var inrange = GetAllPackage().Where(x => x.Threshold <= amount).ToList();
            return inrange.OrderBy(x => x.Threshold).Last();
        }

        public void InsertUserPackage(UserPackage userPackage)
        {
            _uow.Repository<UserPackage>().Insert(userPackage);
        }

        public void SaveChange()
        {
            _uow.Save();
        }
    }
}