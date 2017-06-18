using System.Linq;
using System.Data.Entity;
using System;
using FIX.Service.Entities;

namespace FIX.Service
{
    public class RoleService : EntityService<Role>, IRoleService
    {
        public RoleService(IUnitOfWork uow) : base(uow)
        {
        }

        public Role GetBy(UserProfile userProfile)
        {
            return Get().Where(x => x.RoleId == userProfile.RoleId).FirstOrDefault();
        }
    }
}