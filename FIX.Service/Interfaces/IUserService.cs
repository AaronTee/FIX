using FIX.Service.Entities;
using System.Linq;

namespace FIX.Service
{
    public interface IUserService
    {
        IQueryable<Role> GetAllRoles();
        IQueryable<User> GetAllUsers();
        IQueryable<UserBankAccount> GetAllUserBankAccount(User user);
        User GetUserBy(string username);
        User GetUserBy(int? id);
        bool IsValid(string username, string password);
        bool IsValidEmailAddress(string email);
        void InsertUser(User user);
        void UpdateUser(User user);
        void SaveChanges();
    }
}