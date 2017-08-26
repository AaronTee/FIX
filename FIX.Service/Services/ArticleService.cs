using FIX.Service.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIX.Service
{
    public class ArticleService : IArticleService
    {
        private IUnitOfWork _uow;

        public ArticleService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public void UpdatePost(Post post)
        {
            _uow.Repository<Post>().Update(post);
        }

        public void InsertPost(Post post)
        {
            _uow.Repository<Post>().Insert(post);
        }

        public Post GetPost(int postType)
        {
            return _uow.Repository<Post>().GetAsQueryable(x => x.PostType == postType).FirstOrDefault();
        }

        public bool SaveChange(int userId)
        {
            return _uow.Save(userId);
        }

        
    }
}
