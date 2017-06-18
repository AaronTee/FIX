using System.Linq;
using System.Data.Entity;
using System;
using FIX.Service.Entities;

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

        public bool IsValid(string username, string password)
        {
            return _uow.Repository<User>().GetAsQueryable().Where(x => x.Username == username && x.Password == password).FirstOrDefault() != null;
        }

        public bool IsValidEmailAddress(string email)
        {
            return _uow.Repository<User>().GetAsQueryable().Where(x => x.Email == email).FirstOrDefault() == null;
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