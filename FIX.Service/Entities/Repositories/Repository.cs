using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using FIX.Service.Models;
using FIX.Service.Entities;
using System.Linq.Expressions;
using System.Data.Entity.Core.Objects;

namespace FIX.Service.Models.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        internal DbContext dbContext;
        internal DbSet<TEntity> dbSet;
        protected IQueryable<TEntity> Entities;

        public Repository(DbContext context)
        {
            this.dbContext = context;
            this.dbSet = dbContext.Set<TEntity>();
        }

        protected IList<TReturn> Get<TResult, TKey, TGroup, TReturn>(
        List<Expression<Func<TEntity, bool>>> predicates,
        Expression<Func<TEntity, TResult>> firstSelector,
        Expression<Func<TResult, TKey>> orderSelector,
        Func<TResult, TGroup> groupSelector,
        Func<IGrouping<TGroup, TResult>, TReturn> selector)
        {
            return predicates
                .Aggregate(Entities, (current, predicate) => current.Where(predicate))
                .Select(firstSelector)
                .OrderBy(orderSelector)
                .GroupBy(groupSelector)
                .Select(selector)
                .ToList();
     }


        public IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!String.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public IQueryable<TEntity> GetAsQueryable(
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           string includeProperties = null)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!String.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            if (orderBy != null)
            {
                return orderBy(query);
            }
            else
            {
                return query;
            }
        }

        public TEntity GetByKey(object key)
        {
            return dbSet.Find(key);
        }

        public void Insert(TEntity entity)
        {
            try
            {
                dbSet.Add(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        public void UpdateAll(IEnumerable<TEntity> entities)
        {
            foreach (TEntity entity in entities)
            {
                var dbEntityEntry = dbContext.Entry(entity);
                foreach (var property in dbEntityEntry.OriginalValues.PropertyNames)
                {
                    var original = dbEntityEntry.OriginalValues.GetValue<object>(property);
                    var current = dbEntityEntry.CurrentValues.GetValue<object>(property);
                    if (original != null && !original.Equals(current))
                        dbEntityEntry.Property(property).IsModified = true;
                }
            }
        }

        public void Update(TEntity entity)
        {
            try
            {
                var dbEntityEntry = dbContext.Entry(entity);
                foreach (var property in dbEntityEntry.OriginalValues.PropertyNames)
                {
                    var original = dbEntityEntry.OriginalValues.GetValue<object>(property);
                    var current = dbEntityEntry.CurrentValues.GetValue<object>(property);
                    if (original != null && !original.Equals(current))
                        dbEntityEntry.Property(property).IsModified = true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void Delete(object key)
        {
            TEntity entityToDelete = dbSet.Find(key);
            Delete(entityToDelete);
        }

        public void DeleteAll(IEnumerable<TEntity> entities)
        {
            foreach (TEntity entity in entities)
            {
                if (dbContext.Entry(entity).State == EntityState.Detached)
                {
                    dbSet.Attach(entity);
                }
                dbSet.Remove(entity);
            }
        }

        public void Delete(TEntity entity)
        {
            try
            {
                if (dbContext.Entry(entity).State == EntityState.Detached)
                {
                    dbSet.Attach(entity);
                }
                dbSet.Remove(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        public int Count(Expression<Func<TEntity, bool>> filter = null)
        {
            int count = 0;
            IEnumerable<TEntity> result;

            result = (filter != null) ? dbSet.Where(filter) : dbSet;

            if (result != null)
            {
                count = result.OrderBy(m => 0).Count();
            }

            return count;
        }

        //Stored Procedure
        public IEnumerable<TEntity> ExecWithStoreProcedure(string query, params object[] paramVal)
        {
            return dbContext.Database.SqlQuery<TEntity>("EXEC " + query, paramVal).ToList();
        }
    }
}