using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FIX.Service
{
    public interface IRepository<TEntity> where TEntity : class
    {
        int Count(Expression<Func<TEntity, bool>> filter = null);
        void Delete(object key);
        void Delete(TEntity entity);
        void DeleteAll(IEnumerable<TEntity> entities);
        IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = null);
        IQueryable<TEntity> GetAsQueryable(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = null);
        TEntity GetByKey(object key);
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void UpdateAll(IEnumerable<TEntity> entities);
        IEnumerable<TEntity> ExecWithStoreProcedure(string query, params object[] paramVal);
    }
}