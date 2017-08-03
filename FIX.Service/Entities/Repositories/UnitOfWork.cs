using FIX.Service.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;

namespace FIX.Service.Models.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected FIXEntities _context = null;
        public Dictionary<Type, object> repositories;
        private bool disposed = false;
        private DbContextTransaction _transaction { get; set; }

        public UnitOfWork()
        {
            _context = DependencyResolver.Current.GetService<FIXEntities>();
            repositories = new Dictionary<Type, object>();
        }

        public FIXEntities Context
        {
            get { return _context; }
        }

        public IRepository<T> Repository<T>() where T : class
        {
            if (repositories.Keys.Contains(typeof(T)) == true)
            {
                return repositories[typeof(T)] as IRepository<T>;
            }
            IRepository<T> repo = new Repository<T>(_context);
            repositories.Add(typeof(T), repo);
            return repo;
        }

        public IUnitOfWork BeginTransaction()
        {
            _transaction = _context.Database.BeginTransaction();
            return this;
        }

        public bool EndTransaction(int userId, bool goAsync = false)
        {
            try
            {
                Save(userId, goAsync);
                _transaction.Commit();
            }
            catch (DbEntityValidationException ex)
            {
                Log.Error(ex.Message, ex);
            }

            return true;
        }

        public bool Save(int userId, bool goAsync = false)
        {
            for (int i = 0; i < DBConstant.MAX_CONCURRENCY_ITERATION; i++)
            {
                try
                {
                    _context.SaveChanges(userId, goAsync);
                    return true;
                }
                catch (DbEntityValidationException ve)
                {
                    var error = ve.EntityValidationErrors.First().ValidationErrors.First();
                    Log.Error(error.ErrorMessage);
                    var msg = String.Format("Validation Error :: {0} - {1}", error.PropertyName, error.ErrorMessage);
                    throw;
                }
                catch (OptimisticConcurrencyException ex)
                {
                    Log.Warn("Concurrency detected conflict, refreshing rowversion", ex);
                    foreach (var entity in _context.ChangeTracker.Entries())
                    {
                        entity.Reload();
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var entity in ex.Entries)
                    {
                        entity.Reload(); //client wins
                    }
                    Log.Warn("Concurrency detected conflict, refreshing rowversion", ex);
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message, ex);
                    return false;
                }
            }
            return false;
        }

        public bool Save()
        {
            for (int i = 0; i < DBConstant.MAX_CONCURRENCY_ITERATION; i++)
            {
                try
                {
                    _context.SaveChanges();
                    return true;
                }
                catch (OptimisticConcurrencyException ex)
                {
                    Log.Warn("Concurrency detected conflict, refreshing rowversion...", ex);
                    foreach (var entity in _context.ChangeTracker.Entries())
                    {
                        entity.Reload();
                    }
                }
                catch (DbEntityValidationException ve)
                {
                    var error = ve.EntityValidationErrors.First().ValidationErrors.First();
                    Log.Error(error.ErrorMessage);
                    var msg = String.Format("Validation Error :: {0} - {1}", error.PropertyName, error.ErrorMessage);
                    throw;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message, ex);
                    return false;
                }
            }
            return false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}