using FIX.Service.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FIX.Service
{
    public interface IUserService
    {
        IQueryable<Role> GetAllRoles();
        Role GetUserRoleBy(UserProfile userProfile);
        IQueryable<User> GetAllUsers();
        IQueryable<UserBankAccount> GetAllUserBankAccount(User user);
        User GetUserBy(string username);
        User GetUserBy(int? id);
        Task<bool> IsValid(string username, string password);
        bool IsValidEmailAddress(string email);
        Guid AssignNewValidationCode(User user);
        bool ValidateActivationCode(Guid activationCode);
        IQueryable<User> GetReferralChildren(int? id);
        IQueryable<User> GetUsersWithoutAdmin();
        void InsertUser(User user);
        void UpdateUser(User user);
        void SaveChanges();
        void SaveChanges(int userId);
    }
}