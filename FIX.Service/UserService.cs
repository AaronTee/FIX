using System.Linq;
using FIX.Core.Data;
using FIX.Data;
using FIX.Service.Interface;

namespace FIX.Service
{
    public class UserService : IUserService
    {
        private IRepository<User> _userRepo;
        private IRepository<UserProfile> _userProfileRepo;
        private IRepository<Role> _roleRepo;
        private IRepository<UserBankAccount> _userBankAccountRepo;
        private IRepository<Gender> _genderRepo;

        public UserService(IRepository<User> userRepo, IRepository<UserProfile> userProfileRepo, IRepository<Role> roleRepo
            , IRepository<UserBankAccount> userBankAccountRepo, IRepository<Gender> genderRepo)
        {
            _userRepo = userRepo;
            _userProfileRepo = userProfileRepo;
            _roleRepo = roleRepo;
            _userBankAccountRepo = userBankAccountRepo;
            _genderRepo = genderRepo;
        }

        public IQueryable<User> GetAllUsers()
        {
            return _userRepo.Table;
        }

        public bool IsValid(string username, string password)
        {
            var user = GetAllUsers().Where(x => x.Username == username && x.Password == password).FirstOrDefault();
            return user != null;
        }

        public User GetUser(int id)
        {
            return _userRepo.GetById(id);
        }

        public int GetUserID(string username)
        {
            return GetAllUsers().Where(x => x.Username == username).First().UserId;
        }

        public User GetUser(string username, string password)
        {
            return GetAllUsers().Where(x => x.Username == username && x.Password == password).FirstOrDefault();
        }

        public void InsertUser(User user)
        {
            _userRepo.Insert(user);
        }

        public void UpdateUser(User user)
        {
            _userRepo.Update(user);
        }

        public void DeleteUser(User user)
        {
            _userRepo.Delete(user);
        }

        public IQueryable<Role> GetAllRoles()
        {
            return _roleRepo.Table;
        }

        public Role GetRoleById(int id)
        {
            return _roleRepo.GetById(id);
        }

        public IQueryable<UserBankAccount> GetAllBankAccount()
        {
            return _userBankAccountRepo.Table;
        }

        public UserBankAccount GetPrimaryBankAccount(User user)
        {
            return GetAllBankAccount().Where(x => x.IsPrimary && x.UserId == user.UserId).FirstOrDefault();
        }

        public IQueryable<Gender> GetAllGender()
        {
            return _genderRepo.Table;
        }

        public Gender GetGenderById(int id)
        {
            return _genderRepo.GetById(id);
        }
    }
}