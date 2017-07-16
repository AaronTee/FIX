using System.Linq;
using System.Data.Entity;
using System;
using FIX.Service.Entities;

namespace FIX.Service
{
    public class RoleService : IRoleService
    {
        private IRepository<Role> _roleService;

        public RoleService(IUnitOfWork uow)
        {
            _roleService = uow.Repository<Role>();
        }

        public Role GetBy(UserProfile userProfile)
        {
            return _roleService.Get().Where(x => x.RoleId == userProfile.RoleId).FirstOrDefault();
        }
    }
}