using System;
using System.Linq;
using FIX.Core.Data;
using FIX.Data;
using FIX.Service.Interface;

namespace FIX.Service
{
    public class RoleService : IRoleService
    {
        private IRepository<Role> roleRepo;

        public RoleService(IRepository<Role> roleRepository)
        {
            this.roleRepo = roleRepository;
        }

        public IQueryable<Role> GetAsQueryable()
        {
            return roleRepo.Table;
        }

        public Role GetById(int id)
        {
            return roleRepo.GetById(id);
        }

        public void Insert(Role entity)
        {
            roleRepo.Insert(entity);
        }

        public void Update(Role entity)
        {
            roleRepo.Update(entity);
        }

        public void Delete(Role entity)
        {
            roleRepo.Delete(entity);
        }
    }
}