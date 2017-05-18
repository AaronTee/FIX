using System.Data.Entity.ModelConfiguration;
using FIX.Core.Data;

namespace FIX.Data.Mapping
{
    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            //key
            HasKey(t => t.ID);
            //properties
            Property(t => t.UserName).IsRequired();
            Property(t => t.Email).IsRequired();
            Property(t => t.Password).IsRequired();
            Property(t => t.IP).IsRequired();
            Property(t => t.CreatedTimestamp).IsRequired();
            Property(t => t.ModifiedTimestamp).IsRequired();
            //table
            ToTable("Users");
        }
    }
}