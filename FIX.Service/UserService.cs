using System.Linq;
using FIX.Core.Data;
using FIX.Data;
using System.Data.Entity;
using FIX.Service.Interface;

namespace FIX.Service
{
    public class UserService : IUserService
    {
        private IDbContext _context;

        public UserService(IDbContext context)
        {
            _context = context;
        }

        public IQueryable<User> GetAllUsers()
        {
            var a = _context.Set<User>();
            return _context.Set<User>();
        }

        public bool IsValid(string username, string password)
        {
            var user = GetAllUsers().Where(x => x.Username == username && x.Password == password).FirstOrDefault();
            return user != null;
        }

        public int GetUserID(string username)
        {
            return GetAllUsers().Where(x => x.Username == username).First().UserId;
        }

        public User GetUser(int id)
        {
            return _context.Set<User>().Find(id);
        }

        public User GetUser(string username, string password)
        {
            return GetAllUsers().Where(x => x.Username == username && x.Password == password).FirstOrDefault();
        }

        public void InsertUser(User user)
        {
            _context.Insert(user);
        }

        public void UpdateUser(User user)
        {
            _context.Update(user);
        }

        public void DeleteUser(User user)
        {
            _context.Delete(user);
        }

        public IQueryable<Role> GetAllRoles()
        {
            return _context.Set<Role>();
        }

        public Role GetRoleById(int id)
        {
            return _context.Set<Role>().Find(id);
        }

        public IQueryable<UserBankAccount> GetAllBankAccount()
        {
            return _context.Set<UserBankAccount>();
        }

        public UserBankAccount GetPrimaryBankAccount(User user)
        {
            return GetAllBankAccount().Where(x => x.IsPrimary && x.UserId == user.UserId).FirstOrDefault();
        }

        public IQueryable<Gender> GetAllGender()
        {
            return _context.Set<Gender>();
        }

        public Gender GetGenderById(int id)
        {
            return _context.Set<Gender>().Find(id);
        }
    }
}