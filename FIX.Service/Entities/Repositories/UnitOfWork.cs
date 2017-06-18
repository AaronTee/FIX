using FIX.Service.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;

namespace FIX.Service.Models.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        #region Defination
        readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected FIXEntities _context = null;
        public Dictionary<Type, object> repositories;
        private bool disposed = false;

        #endregion
        #region Contructor
        public UnitOfWork()
        {
            _context = DependencyResolver.Current.GetService<FIXEntities>();
            repositories = new Dictionary<Type, object>();
        }
        #endregion
        #region Property
        public FIXEntities Context
        {
            get { return _context; }
            //set{ }
        }
        #endregion
        #region Public Functions

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

        public void Save()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (DbEntityValidationException ve)
            {
                var error = ve.EntityValidationErrors.First().ValidationErrors.First();
                Logger.Error(error.ErrorMessage);
                var msg = String.Format("Validation Error :: {0} - {1}",
                           error.PropertyName, error.ErrorMessage);
                throw;
            }
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this._context.Dispose();
                }
            }

            this.disposed = true;
        }
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}