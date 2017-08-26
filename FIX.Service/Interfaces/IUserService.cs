using FIX.Service.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using static FIX.Service.DBConstant;

namespace FIX.Service
{
    public interface IUserService
    {
        User ActivateUserAccount(string tokenString);
        bool ResetPassword(string tokenString, string newPassword);
        string CreateNewToken(User user, DBConstant.EAccessTokenPurpose purpose);
        IQueryable<Role> GetAllRoles();
        IQueryable<UserBankAccount> GetAllUserBankAccount(int? userId);
        IQueryable<User> GetAllUsers();
        IQueryable<User> GetAllUsersWithoutAdmin();
        User GetReferralBy(int? id);
        IQueryable<User> GetReferralChildren(int? id);
        User GetUserBy(string username);
        User GetUserBy(int? id);
        User GetUserByEmail(string email);
        Role GetUserRoleBy(UserProfile userProfile);
        IQueryable<User> GetUsersWithoutAdmin();
        void InsertUser(User user);
        Task<bool> IsValid(string username, string password, string keyPhrase);
        bool IsValidEmailAddress(string email);
        AccessToken IsValidToken(string tokenString, DBConstant.EAccessTokenPurpose purpose);
        void SaveChanges();
        bool SaveChanges(int userId);
        void UpdateUser(User user);
    }
}