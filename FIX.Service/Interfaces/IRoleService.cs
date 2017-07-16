using FIX.Service.Entities;

namespace FIX.Service
{
    public interface IRoleService
    {
        Role GetBy(UserProfile userProfile);
    }
}