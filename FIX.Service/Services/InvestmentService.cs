using System.Linq;
using System.Data.Entity;
using System;
using FIX.Service.Entities;
using System.Threading.Tasks;
using static FIX.Service.DBConstant;
using System.Collections.Generic;

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
            return _uow.Repository<UserPackage>().GetAsQueryable(filter: x => x.UserId == userId);
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

            if(inrange.Count > 0)
            {
                return inrange.OrderBy(x => x.Threshold).Last();
            }

            return new Package
            {
                Description = "N/A",
                Rate = 0
            };
        }

        public IQueryable<vwPendingReturnInvestor_Test> GetAllPendingReturn()
        {
            return _uow.Repository<vwPendingReturnInvestor_Test>().GetAsQueryable();
        }

        public IEnumerable<spMatchingBonus_Result> GetMatchingBonusResult(int? userId)
        {

            if (userId != null)
            {
                var param1 = new System.Data.SqlClient.SqlParameter("level", 2);
                var param2 = new System.Data.SqlClient.SqlParameter("userId", userId);

                var result = _uow.Repository<spMatchingBonus_Result>().ExecWithStoreProcedure("spMatchingBonus @level, @userId", new[]
                {
                    param1, param2
                });

                return result;
            }

            throw new Exception("userId cannot be null");
        }

        public UserPackageDetail GetUserPackageDetail(int UPDId)
        {
            return _uow.Repository<UserPackageDetail>().GetByKey(UPDId);
        }

        public void UpdateUserPackageDetail(UserPackageDetail upd)
        {
            _uow.Repository<UserPackageDetail>().Update(upd);
        }

        public void InsertUserPackage(UserPackage userPackage)
        {
            _uow.Repository<UserPackage>().Insert(userPackage);
        }

        public void SaveChange()
        {
            _uow.Save();
        }

        public void SaveChange(int userId)
        {
            _uow.Save(userId);
        }
    }
}