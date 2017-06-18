using FIX.Service.Entities;
using System;

namespace FIX.Service
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> Repository<T>() where T : class;
        void Save();
    }
}