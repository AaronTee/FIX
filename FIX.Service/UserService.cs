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
        private IRepository<UserBankAccount> _userBankAccountRepository;

        public UserService(IRepository<User> userRepository, IRepository<UserProfile> userProfileRepository, IRepository<Role> roleRepository
            , IRepository<UserBankAccount> userBankAccountRepository)
        {
            _userRepo = userRepository;
            _userProfileRepo = userProfileRepository;
            _roleRepo = roleRepository;
            _userBankAccountRepository = userBankAccountRepository;
        }

        public IQueryable<User> GetUsers()
        {
            return _userRepo.Table;
        }

        public bool IsValid(string username, string password)
        {
            var user = GetUsers().Where(x => x.Username == username && x.Password == password).FirstOrDefault();
            return user != null;
        }

        public User GetUser(int id)
        {
            return _userRepo.GetById(id);
        }

        public int GetUserID(string username)
        {
            return GetUsers().Where(x => x.Username == username).First().UserId;
        }

        public User GetUser(string username, string password)
        {
            return GetUsers().Where(x => x.Username == username && x.Password == password).FirstOrDefault();
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
    }
}