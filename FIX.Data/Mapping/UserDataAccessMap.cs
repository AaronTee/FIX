using System.Data.Entity.ModelConfiguration;
using FIX.Core.Data;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIX.Data.Mapping
{
    public class UserDataAccessMap : EntityTypeConfiguration<UserDataAccess>
    {
        public UserDataAccessMap()
        {
            ////key
            //HasKey(t => t.ID);
            ////properties
            //Property(t => t.AllowCreate).IsRequired();
            //Property(t => t.AllowDelete).IsRequired();
            //Property(t => t.AllowEdit).IsRequired();
            //Property(t => t.AllowView).IsRequired();
            //Property(t => t.AllowApprove).IsRequired();
            //Property(t => t.CreatedTimestamp).IsRequired();
            //Property(t => t.ModifiedTimestamp);
            ////table
            //ToTable("UserDataAccess");
        }
    }
}