using System.Data.Entity;
using FIX.Core;
using System;
using System.Data.Entity.Infrastructure;

namespace FIX.Data
{
    public interface IDbContext
    {
        void Delete<TEntity>(TEntity entity) where TEntity : class;
        void Insert<TEntity>(TEntity entity) where TEntity : class;
        int SaveChanges();
        IDbSet<TEntity> Set<TEntity>() where TEntity : class;
        void Update<TEntity>(TEntity entity, bool AddIfNotExist = true) where TEntity : class;
    }
}