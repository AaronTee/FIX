using System.Linq;
using FIX.Service.Entities;
using System.Collections.Generic;

namespace FIX.Service
{
    public interface IInvestmentService
    {
        IQueryable<Package> GetAllPackage();
        IQueryable<vwPendingReturnInvestor_Test> GetAllPendingReturn();
        IQueryable<ReturnInterest> GetAllReturnInterest(int userPackageId);
        IQueryable<UserPackage> GetAllUserPackage(int userId);
        Package GetEntitledPackage(decimal amount);
        MatchingBonus GetMatchingBonus(int? matchingBonusId);
        decimal GetMatchingBonusAmount(int? userId, int? MatchingBonusId);
        IQueryable<MatchingBonus> GetMatchingBonusList(int? userId);
        decimal GetPackageRate(int PackageId);
        ReturnInterest GetReturnInterest(int UPDId);
        UserPackage GetUserPackage(int UPId);
        void InsertMatchingBonus(MatchingBonus matchingBonus);
        void InsertReturnInvestment(ReturnInterest returnInterest);
        void InsertUserPackage(UserPackage userPackage);
        void SaveChange();
        void SaveChange(int userId);
        void UpdateReturnInterest(ReturnInterest upd);
        void UpdateUserPackage(UserPackage up);
    }
}