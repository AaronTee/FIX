using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIX.Service
{
    public abstract class BaseService : IBaseService
    {
        protected IUnitOfWork _uow;

        public BaseService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public virtual void StartTransaction()
        {
            _uow.BeginTransaction();
        }

        public virtual void EndTransaction(int userId, bool goAsync = false)
        {
            _uow.EndTransaction(userId, goAsync);
        }
    }
}
