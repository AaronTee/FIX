using System.Linq;
using System.Data.Entity;
using System;
using FIX.Service.Entities;

namespace FIX.Service
{
    public class UserProfileService : EntityService<UserProfile>, IUserProfileService
    {
        public UserProfileService(IUnitOfWork uow) : base(uow)
        {
        }

        public UserProfile GetBy(int UserId)
        {
            return Get().Where(x => x.UserId == UserId).FirstOrDefault();
        }
    }
}