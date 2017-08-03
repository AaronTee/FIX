using System.Linq;
using FIX.Service.Entities;
using System.Collections.Generic;

namespace FIX.Service
{
    public interface IInvestmentService
    {
        IQueryable<Package> GetAllPackage();
        IQueryable<vwPendingReturnInvestor_Test> GetAllPendingReturn();
        IQueryable<UserPackage> GetAllUserPackage(int userId);
        IQueryable<UserPackageDetail> GetAllUserPackageDetail(int userPackageId);
        Package GetEntitledPackage(decimal amount);
        decimal GetMatchingBonusAmount(int? userId, int? MatchingBonusId);
        IQueryable<MatchingBonus> GetMatchingBonusList(int? userId);
        MatchingBonus GetMatchingBonus(int? matchingBonusId);
        decimal GetPackageRate(int PackageId);
        UserPackage GetUserPackage(int UPId);
        UserPackageDetail GetUserPackageDetail(int UPDId);
        void InsertUserPackage(UserPackage userPackage);
        void SaveChange();
        void SaveChange(int userId);
        void UpdateUserPackageDetail(UserPackageDetail upd);
    }
}