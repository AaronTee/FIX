using System.Linq;
using FIX.Core.Data;
using FIX.Data;
using FIX.Service.Interface;

namespace FIX.Service
{
    public class UserService : IUserService
    {
        private IRepository<User> userRepo;
        private IRepository<UserProfile> userProfileRepo;

        public UserService(IRepository<User> userRepository, IRepository<UserProfile> userProfileRepository)
        {
            this.userRepo = userRepository;
            this.userProfileRepo = userProfileRepository;
        }

        public IQueryable<User> GetUsers()
        {
            return userRepo.Table;
        }

        public bool IsValid(string username, string password)
        {
            var user = GetUsers().Where(x => x.Username == username && x.Password == password).FirstOrDefault();
            return user != null;
        }

        public User GetUser(long id)
        {
            return userRepo.GetById(id);
        }

        public void InsertUser(User user)
        {
            userRepo.Insert(user);
        }

        public void UpdateUser(User user)
        {
            userRepo.Update(user);
        }

        public void DeleteUser(User user)
        {
            userProfileRepo.Delete(user.UserProfile);
            userRepo.Delete(user);
        }
    }
}