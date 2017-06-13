using System.Linq;
using FIX.Core.Data;

namespace FIX.Service.Interface
{
    public interface IUserService : IBaseService
    {
        IQueryable<User> GetUsers();
        User GetUser(int id);
        int GetUserID(string username);
        User GetUser(string username, string password);
        void InsertUser(User user);
        void UpdateUser(User user);
        void DeleteUser(User user);
        bool IsValid(string username, string password);
    }
}