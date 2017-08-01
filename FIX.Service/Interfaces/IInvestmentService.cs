using System.Linq;
using FIX.Service.Entities;
using System.Collections.Generic;

namespace FIX.Service
{
    public interface IInvestmentService/* : IBaseService*/
    {
        IQueryable<Package> GetAllPackage();
        IQueryable<vwPendingReturnInvestor_Test> GetAllPendingReturn();
        IQueryable<UserPackage> GetAllUserPackage(int userId);
        IQueryable<UserPackageDetail> GetAllUserPackageDetail(int userPackageId);
        Package GetEntitledPackage(decimal amount);
        double GetMatchingBonusAmount(int? userId, int? UPDId);
        IQueryable<MatchingBonus> GetMatchingBonusList(int? userId);
        decimal GetPackageRate(int PackageId);
        UserPackage GetUserPackage(int UPId);
        UserPackageDetail GetUserPackageDetail(int UPDId);
        void InsertUserPackage(UserPackage userPackage);
        void SaveChange();
        void SaveChange(int userId);
        void UpdateUserPackageDetail(UserPackageDetail upd);
    }
}