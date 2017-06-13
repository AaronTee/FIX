using System.Data.Entity.ModelConfiguration;
using FIX.Core.Data;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIX.Data.Mapping
{
    public class RoleMap : EntityTypeConfiguration<Role>
    {
        public RoleMap()
        {
            //key
            HasKey(t => t.RoleId).Property(p => p.RoleId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //properties
            Property(t => t.RoleName).IsRequired();
            Property(t => t.Description).IsOptional();
            Property(t => t.CreatedTimestamp).IsRequired();
            Property(t => t.ModifiedTimestamp);
            //table
            ToTable("Role");
        }
    }
}