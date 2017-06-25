using System.Linq;
using System.Data.Entity;
using System;
using FIX.Service.Entities;
using System.Threading.Tasks;
using static FIX.Service.DBConstant;

namespace FIX.Service
{
    public class UserService : IUserService
    {

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

        public IQueryable<UserBankAccount> GetAllUserBankAccount(User user)
        {
            return _uow.Repository<UserBankAccount>().GetAsQueryable().Where(x => x.UserId == user.UserId);
        }

        public User GetUserBy(string username)
        {
           return _uow.Repository<User>().GetAsQueryable().Where(x => x.Username == username).FirstOrDefault();
        }

        public User GetUserBy(int? id)
        {
            return _uow.Repository<User>().GetAsQueryable().Where(x => x.UserId == id).FirstOrDefault();
        }

        public async Task<bool> IsValid(string username, string password)
        {
            var task = Task.Run(() => _uow.Repository<User>().GetAsQueryable().Where(x => x.Username == username && x.Password == password).FirstOrDefault() != null);
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
                ActivationCode = activationCode
            };
            _uow.Repository<UserActivation>().Insert(newActivation);

            return activationCode;
        }

        public bool ValidateActivationCode(Guid activationCode)
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
                    return true;
                }
            }

            return false;
        }

        public void InsertUser(User user)
        {
            _uow.Repository<User>().Insert(user);
        }

        public void UpdateUser(User user)
        {
            _uow.Repository<User>().Update(user);
        }

        public void SaveChanges()
        {
            _uow.Save();
        }

    }
}