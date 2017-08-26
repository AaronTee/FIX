using FIX.Service.Entities;

namespace FIX.Service
{
    public interface IArticleService
    {
        Post GetPost(int postType);
        bool SaveChange(int userId);
        void UpdatePost(Post post);
        void InsertPost(Post post);
    }
}