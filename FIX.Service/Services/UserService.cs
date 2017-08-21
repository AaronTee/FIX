using System.Linq;
using System.Data.Entity;
using System;
using FIX.Service.Entities;
using System.Threading.Tasks;
using static FIX.Service.DBConstant;
using FIX.Service.Utils;
using System.Configuration;

namespace FIX.Service
{
    public class UserService : IUserService
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IUnitOfWork _uow;

        public UserService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public IQueryable<Role> GetAllRoles()
        {
            return _uow.Repository<Role>().GetAsQueryable();
        }

        public Role GetUserRoleBy(UserProfile userProfile)
        {
            return _uow.Repository<Role>().GetByKey(userProfile.RoleId);
        }

        public IQueryable<User> GetAllUsers()
        {
            return _uow.Repository<User>().GetAsQueryable();
        }

        public IQueryable<User> GetAllUsersWithoutAdmin()
        {
            return _uow.Repository<User>().GetAsQueryable(filter: x=> x.UserProfile.Role.Description != DBCRole.Admin);
        }

        public IQueryable<UserBankAccount> GetAllUserBankAccount(int? userId)
        {
            return _uow.Repository<UserBankAccount>().GetAsQueryable().Where(x => x.UserId == userId);
        }

        public User GetUserBy(string username)
        {
           return _uow.Repository<User>().GetAsQueryable().Where(x => x.Username == username).FirstOrDefault();
        }

        public User GetReferralBy(int? id)
        {
            var referralId = _uow.Repository<UserProfile>().GetByKey(id).ReferralId;
            return GetUserBy(referralId);
        }

        public User GetUserBy(int? id)
        {
            return _uow.Repository<User>().GetAsQueryable().Where(x => x.UserId == id).FirstOrDefault();
        }

        public async Task<bool> IsValid(string username, string password, string keyPhrase)
        {
            var task = Task.Run(() =>
            {
                var user = _uow.Repository<User>().GetAsQueryable().Where(x => x.Username == username && x.StatusId != (int)EStatus.Deactivated).FirstOrDefault();
                if (user != null)
                {
                    try
                    {
                        return SecurePasswordHasher.Verify(password, user.Password);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message, ex);
                        return false;
                    }
                }
                return false;
            });
            return await task;
        }

        public bool IsValidEmailAddress(string email)
        {
            return _uow.Repository<User>().GetAsQueryable().Where(x => x.Email == email).FirstOrDefault() == null;
        }

        public Guid AssignNewValidationCode(User user)
        {
            Guid activationCode = Guid.NewGuid();

            if (user.UserActivation != null) _uow.Repository<UserActivation>().Delete(user.UserActivation);

            var newActivation = new UserActivation
            {
                UserId = user.UserId,
                ActivationCode = activationCode,
                StatusId = (int)EStatus.Active,
                ExpiredTimestamp = DateTime.UtcNow
            };
            _uow.Repository<UserActivation>().Insert(newActivation);

            return activationCode;
        }

        public bool IsValidActivationCode(Guid activationCode)
        {
            var userActivation = _uow.Repository<UserActivation>().GetAsQueryable().Where(x => x.ActivationCode == activationCode && x.StatusId == (int)EStatus.Active).FirstOrDefault();

            if (userActivation != null)
            {
                var user = _uow.Repository<User>().GetAsQueryable().Where(x => x.UserId == userActivation.UserId).FirstOrDefault();

                if (user != null)
                {
                    return true;
                }
            }

            return false;
        }

        public User ValidateActivationCode(Guid activationCode)
        {
            var userActivation = _uow.Repository<UserActivation>().GetAsQueryable().Where(x => x.ActivationCode == activationCode && x.StatusId == (int)EStatus.Active).FirstOrDefault();

            if(userActivation != null)
            {
                var user = _uow.Repository<User>().GetAsQueryable().Where(x => x.UserId == userActivation.UserId).FirstOrDefault();

                if(user != null)
                {
                    user.HasEmailVerified = true;
                    _uow.Repository<UserActivation>().Delete(userActivation);
                    SaveChanges();
                    return user;
                }
            }

            return null;
        }

        public IQueryable<User> GetReferralChildren(int? id)
        {
            return GetAllUsers().Where(x => x.UserProfile.ReferralId == id);
        }

        public IQueryable<User> GetUsersWithoutAdmin()
        {
            return GetAllUsers().Where(x => x.UserProfile.RoleId != (int)DBCRole.Id.Admin);
        }

        public void InsertUser(User user)
        {
            user.Password = SecurePasswordHasher.Hash(user.Password);
            _uow.Repository<User>().Insert(user);
        }

        public void UpdateUser(User user)
        {
            _uow.Repository<User>().Update(user);
        }

        public void ResetPassword(int? userId, string rawPassword, string keyPhrase)
        {
            _uow.Repository<User>().GetByKey(userId).Password = StringCipher.Encrypt(rawPassword, keyPhrase);
        }

        public bool SaveChanges(int userId)
        {
            return _uow.Save(userId);
        }

        public void SaveChanges()
        {
            _uow.Save();
        }

    }
}