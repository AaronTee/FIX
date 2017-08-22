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

        public IQueryable<UserPackage> GetAllPendingUserPackage(int? userId)
        {
            var pendingUserPackages = _uow.Repository<UserPackage>().GetAsQueryable(filter: x => x.StatusId == (int)EStatus.Pending);
            if(userId != 0 && userId != null)
            {
                pendingUserPackages = pendingUserPackages.Where(x => x.UserId == userId);
            }

            return pendingUserPackages;
        }

        public IQueryable<ReturnInterest> GetAllReturnInterest(int userPackageId)
        {
            return _uow.Repository<ReturnInterest>().GetAsQueryable(filter: x => x.UserPackageId == userPackageId);
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

        public IQueryable<MatchingBonus> GetMatchingBonusList(int? userId)
        {
            if (userId != null)
            {
                return _uow.Repository<MatchingBonus>().GetAsQueryable(filter: x => x.ReferralId == userId);
            }

            throw new Exception("userId cannot be null");
        }

        public ReturnInterest GetReturnInterest(int UPDId)
        {
            return _uow.Repository<ReturnInterest>().GetByKey(UPDId);
        }

        public void UpdateReturnInterest(ReturnInterest upd)
        {
            _uow.Repository<ReturnInterest>().Update(upd);
        }

        public void UpdateUserPackage(UserPackage up)
        {
            _uow.Repository<UserPackage>().Update(up);
        }

        public void InsertUserPackage(UserPackage userPackage)
        {
            _uow.Repository<UserPackage>().Insert(userPackage);
        }

        public void InsertReturnInvestment(ReturnInterest returnInterest)
        {
            _uow.Repository<ReturnInterest>().Insert(returnInterest);
        }

        public void InsertMatchingBonus(MatchingBonus matchingBonus)
        {
            _uow.Repository<MatchingBonus>().Insert(matchingBonus);
        }

        public void SaveChange()
        {
            _uow.Save();
        }

        public bool SaveChange(int userId)
        {
            return _uow.Save(userId);
        }

        public UserPackage GetUserPackage(int UPId)
        {
            return _uow.Repository<UserPackage>().GetByKey(UPId);
        }

        public decimal GetMatchingBonusAmount(int? userId, int? MatchingBonusId)
        {
            if (userId != null && MatchingBonusId != null)
            {
                return _uow.Repository<MatchingBonus>().GetByKey(MatchingBonusId).BonusAmount;
            }

            throw new Exception("userId and MatchingBonusId cannot be null");
        }

        public MatchingBonus GetMatchingBonus(int? matchingBonusId)
        {
            return _uow.Repository<MatchingBonus>().GetByKey(matchingBonusId);
        }
    }
}