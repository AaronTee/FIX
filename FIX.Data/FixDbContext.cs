using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Reflection;
using FIX.Core;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Text;

namespace FIX.Data
{
    public partial class FIXDbContext : DbContext, IDbContext
    {
        public FIXDbContext()
            : base("name=FIXConnectionString")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            var typesToRegister = Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => !String.IsNullOrEmpty(type.Namespace))
            .Where(type => type.BaseType != null && type.BaseType.IsGenericType &&
            type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>));
            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.Configurations.Add(configurationInstance);
            }
            base.OnModelCreating(modelBuilder);
        }

        public new IDbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        public IDbSet<TEntity> Get<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        public void Insert<TEntity>(TEntity entity) where TEntity : class
        {
            Set<TEntity>().Add(entity);
            base.Entry(entity).State = EntityState.Added;

        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            Set<TEntity>().Remove(entity);
            base.Entry(entity).State = EntityState.Deleted;

        }

        public void Update<TEntity>(TEntity entity, bool AddIfNotExist = true) where TEntity : class
        {
            if(entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            base.Entry(entity).State = EntityState.Modified;

        }
    }
}