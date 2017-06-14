using System.Data.Entity.ModelConfiguration;
using FIX.Core.Data;

namespace FIX.Data.Mapping
{
    public class UserProfileMap : EntityTypeConfiguration<UserProfile>
    {
        public UserProfileMap()
        {
            //key
            HasKey(t => t.UserProfileId);
            //properties
            Property(t => t.FirstName).IsRequired().HasMaxLength(100).HasColumnType("nvarchar").IsRequired();
            Property(t => t.LastName).HasMaxLength(100).HasColumnType("nvarchar");
            Property(t => t.Address).HasMaxLength(255).HasColumnType("nvarchar");
            Property(t => t.PhoneNo).HasMaxLength(20).HasColumnType("nvarchar");
            Property(t => t.Country).HasMaxLength(20).HasColumnType("nvarchar");
            Property(t => t.CreatedTimestamp).IsRequired();
            Property(t => t.ModifiedTimestamp);
            //table
            ToTable("UserProfile");
            //relation
            HasRequired(t => t.User).WithRequiredDependent(u => u.UserProfile).WillCascadeOnDelete();
        }
    }
}