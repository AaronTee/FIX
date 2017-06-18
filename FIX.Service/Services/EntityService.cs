using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static FIX.Service.Log;

namespace FIX.Service
{
    public abstract class EntityService<TEntity> : IEntityService<TEntity> where TEntity : class
    {
        protected IUnitOfWork uow;

        public EntityService(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public int Insert(TEntity entity)
        {
            try
            {
                uow.Repository<TEntity>().Insert(entity);
                return 0;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return -1;
            }
        }
        public int Update(TEntity entity)
        {
            try
            {
                uow.Repository<TEntity>().Update(entity);
                return 0;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return -1;
            }
        }
        public int Delete(TEntity entity)
        {
            try
            {
                uow.Repository<TEntity>().Delete(entity);
                return 0;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return -1;
            }
        }

        public int DeleteAll(IEnumerable<TEntity> entities)
        {
            try
            {
                uow.Repository<TEntity>().DeleteAll(entities);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return -1;
            }

            return 0;
        }

        public int UpdateAll(IEnumerable<TEntity> entities)
        {
            try
            {
                uow.Repository<TEntity>().UpdateAll(entities);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return -1;
            }

            return 0;
        }

        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = null)
        {
            return uow.Repository<TEntity>().Get(filter, orderBy, includeProperties);
        }

        public virtual IEnumerable<TEntity> GetAsEnumerable()
        {
            return uow.Repository<TEntity>().Get();
        }

        public virtual IQueryable<TEntity> GetAsQueryable()
        {
            return uow.Repository<TEntity>().GetAsQueryable();
        }

    }
}