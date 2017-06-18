using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FIX.Service
{
    public interface IEntityService<TEntity> where TEntity : class
    {
        int Delete(TEntity entity);
        int DeleteAll(IEnumerable<TEntity> entities);
        IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = null);
        IEnumerable<TEntity> GetAsEnumerable();
        IQueryable<TEntity> GetAsQueryable();
        int Insert(TEntity entity);
        int Update(TEntity entity);
        int UpdateAll(IEnumerable<TEntity> entities);
    }
}