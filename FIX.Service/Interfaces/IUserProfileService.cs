using FIX.Service.Entities;

namespace FIX.Service
{
    public interface IUserProfileService : IEntityService<UserProfile>
    {
        UserProfile GetBy(int UserId);
    }
}