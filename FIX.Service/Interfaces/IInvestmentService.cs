using System.Linq;
using FIX.Service.Entities;

namespace FIX.Service
{
    public interface IInvestmentService
    {
        IQueryable<Package> GetAllPackage();
        IQueryable<UserPackage> GetAllUserPackage(int userId);
        IQueryable<UserPackageDetail> GetAllUserPackageDetail(int userPackageId);
        decimal GetPackageRate(int PackageId);
        void InsertUserPackage(UserPackage userPackage);
        Package GetEntitledPackage(decimal amount);
        IQueryable<vwPendingReturnInvestor_Test> GetAllPendingReturn();
        UserPackageDetail GetUserPackageDetail(int UPDId);
        void UpdateUserPackageDetail(UserPackageDetail upd);
        void SaveChange();
        void SaveChange(int userId);
    }
}