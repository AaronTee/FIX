﻿using FIX.Service.Entities;
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
        IQueryable<User> GetAllUsersWithoutAdmin();
        IQueryable<UserBankAccount> GetAllUserBankAccount(int? userId);
        User GetUserBy(string username);
        User GetUserBy(int? id);
        Task<bool> IsValid(string username, string password, string keyPhrase);
        void ResetPassword(int? userId, string rawPassword, string keyPhrase);
        bool IsValidEmailAddress(string email);
        Guid AssignNewValidationCode(User user);
        bool IsValidActivationCode(Guid activationCode);
        User ValidateActivationCode(Guid activationCode);
        IQueryable<User> GetReferralChildren(int? id);
        User GetReferralBy(int? id);
        IQueryable<User> GetUsersWithoutAdmin();
        void InsertUser(User user);
        void UpdateUser(User user);
        void SaveChanges();
        bool SaveChanges(int userId);
    }
}