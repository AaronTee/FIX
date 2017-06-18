using FIX.Service.Entities;

namespace FIX.Service
{
    public interface IRoleService : IEntityService<Role>
    {
        Role GetBy(UserProfile userProfile);
    }
}