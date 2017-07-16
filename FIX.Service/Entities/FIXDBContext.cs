using FIX.Service;
using FIX.Service.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

public partial class FIXEntities : DbContext
{
    protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    // This is overridden to prevent someone from calling SaveChanges without specifying the user making the change
    public override async Task<int> SaveChangesAsync()
    {
        return await base.SaveChangesAsync();
    }

    public override int SaveChanges()
    {
        return base.SaveChanges();
    }

    public void SaveChanges(int userId, bool goAsync)
    {
        Task<int> t;
        // Get all Added/Deleted/Modified entities (not Unmodified or Detached)
        foreach (var ent in this.ChangeTracker.Entries().Where(p => p.State == EntityState.Added || p.State == EntityState.Deleted || p.State == EntityState.Modified))
        {
            // For each changed record, get the audit record entries and add them
            foreach (AuditLog x in GetAuditRecordsForChange(ent, userId))
            {
                this.Set<AuditLog>().Add(x);
            }
        }

        if (goAsync)
            t = SaveChangesAsync();
        else
            SaveChanges();
    }

    private List<AuditLog> GetAuditRecordsForChange(DbEntityEntry dbEntry, int userId)
    {
        try
        {
            List<AuditLog> result = new List<AuditLog>();

            DateTime changeTime = DateTime.UtcNow;

            var manager = ((IObjectContextAdapter)this).ObjectContext.ObjectStateManager;
            EntitySetBase setBase = manager.GetObjectStateEntry(dbEntry.Entity).EntitySet;
            string[] keyNames = setBase.ElementType.KeyMembers.Select(k => k.Name).ToArray();
            string keyName;
            if (keyNames != null)
            {
                keyName = keyNames.FirstOrDefault();
            }
            else
            {
                keyName = "nokey";
            }

            // Get the Table() attribute, if one exists
            //TableAttribute tableAttr = dbEntry.Entity.GetType().GetCustomAttributes(typeof(TableAttribute), false).SingleOrDefault() as TableAttribute;

            //TableAttribute tableAttr = dbEntry.Entity.GetType().GetCustomAttributes(typeof(TableAttribute), false).SingleOrDefault() as TableAttribute;

            // Get table name (if it has a Table attribute, use that, otherwise get the pluralized name)
            string tableName = ObjectContext.GetObjectType(dbEntry.Entity.GetType()).Name;

            // Get primary key value (If you have more than one key column, this will need to be adjusted)
            //var keyNames = dbEntry.Entity.GetType().GetProperties().Where(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Count() > 0).ToList();

            //string keyName = keyNames[0].Name; //dbEntry.Entity.GetType().GetProperties().Single(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Count() > 0).Name;

            if (dbEntry.State == EntityState.Added)
            {
                // For Inserts, just add the whole record
                // If the entity implements IDescribableEntity, use the description from Describe(), otherwise use ToString()

                foreach (string propertyName in dbEntry.CurrentValues.PropertyNames)
                {
                    result.Add(new AuditLog()
                    {
                        AuditLogId = Guid.NewGuid(),
                        UserId = userId,
                        EventTimestamp = changeTime,
                        EventType = DBConstant.EventType.Added,
                        TableName = tableName,
                        RecordId = dbEntry.CurrentValues.GetValue<object>(keyName).ToString(),
                        ColumnName = propertyName,
                        NewValue = dbEntry.CurrentValues.GetValue<object>(propertyName) == null ? null : dbEntry.CurrentValues.GetValue<object>(propertyName).ToString()
                    });
                }
            }
            else if (dbEntry.State == EntityState.Deleted)
            {
                // Same with deletes, do the whole record, and use either the description from Describe() or ToString()
                result.Add(new AuditLog()
                {
                    AuditLogId = Guid.NewGuid(),
                    UserId = userId,
                    EventTimestamp = changeTime,
                    EventType = DBConstant.EventType.Deleted,
                    TableName = tableName,
                    RecordId = dbEntry.OriginalValues.GetValue<object>(keyName).ToString(),
                    ColumnName = "All"
                });
            }
            else if (dbEntry.State == EntityState.Modified)
            {
                foreach (string propertyName in dbEntry.OriginalValues.PropertyNames)
                {
                    // For updates, we only want to capture the columns that actually changed
                    if (!object.Equals(dbEntry.OriginalValues.GetValue<object>(propertyName), dbEntry.CurrentValues.GetValue<object>(propertyName)))
                    {
                        result.Add(new AuditLog()
                        {
                            AuditLogId = Guid.NewGuid(),
                            UserId = userId,
                            EventTimestamp = changeTime,
                            EventType = DBConstant.EventType.Modified,
                            TableName = tableName,
                            RecordId = dbEntry.OriginalValues.GetValue<object>(keyName).ToString(),
                            ColumnName = propertyName,
                            OriginalValue = dbEntry.OriginalValues.GetValue<object>(propertyName) == null ? null : dbEntry.OriginalValues.GetValue<object>(propertyName).ToString(),
                            NewValue = dbEntry.CurrentValues.GetValue<object>(propertyName) == null ? null : dbEntry.CurrentValues.GetValue<object>(propertyName).ToString()
                        });
                    }
                }
            }
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message, ex);
            return new List<AuditLog>();
        }
    }
}