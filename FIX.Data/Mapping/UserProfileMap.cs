using System.Data.Entity.ModelConfiguration;
using FIX.Core.Data;

namespace FIX.Data.Mapping
{
    public class UserProfileMap : EntityTypeConfiguration<UserProfile>
    {
        public UserProfileMap()
        {
            //key
            HasKey(t => t.ID);
            //properties
            Property(t => t.FirstName).IsRequired().HasMaxLength(100).HasColumnType("nvarchar").IsRequired();
            Property(t => t.LastName).HasMaxLength(100).HasColumnType("nvarchar");
            Property(t => t.Address).HasColumnType("nvarchar");
            Property(t => t.Gender).HasMaxLength(2).HasColumnType("nvarchar");
            Property(t => t.CreatedTimestamp).IsRequired();
            Property(t => t.ModifiedTimestamp);
            //table
            ToTable("UserProfiles");
            //relation
            HasRequired(t => t.User).WithRequiredDependent(u => u.UserProfile);
        }
    }
}