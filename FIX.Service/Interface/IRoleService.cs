using System.Linq;
using FIX.Core.Data;

namespace FIX.Service.Interface
{
    public interface IRoleService : IBaseService
    {
        IQueryable<Role> GetAsQueryable();
        Role GetById(int id);
        void Insert(Role entity);
        void Update(Role entity);
        void Delete(Role entity);
    }
}