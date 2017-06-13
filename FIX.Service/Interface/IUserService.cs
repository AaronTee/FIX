using System.Linq;
using FIX.Core.Data;

namespace FIX.Service.Interface
{
    public interface IUserService : IBaseService
    {
        void DeleteUser(User user);
        IQueryable<Role> GetAllRoles();
        IQueryable<User> GetAllUsers();
        Role GetRoleById(int id);
        User GetUser(int id);
        User GetUser(string username, string password);
        int GetUserID(string username);
        void InsertUser(User user);
        bool IsValid(string username, string password);
        void UpdateUser(User user);
        IQueryable<UserBankAccount> GetAllBankAccount();
        UserBankAccount GetPrimaryBankAccount(User user);
        IQueryable<Gender> GetAllGender();
        Gender GetGenderById(int id);
    }
}