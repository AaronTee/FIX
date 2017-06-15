using System.Data.Entity.ModelConfiguration;
using FIX.Core.Data;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIX.Data.Mapping
{
    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            //key
            HasKey(t => t.UserId).Property(p => p.UserId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //properties
            Property(t => t.Username).IsRequired().HasMaxLength(25)
            .HasColumnAnnotation(
            IndexAnnotation.AnnotationName,
            new IndexAnnotation(new IndexAttribute("IX_Username") { IsUnique = true }));
            Property(t => t.Email).IsRequired();
            Property(t => t.Password).IsRequired();
            Property(t => t.IP);
            Property(t => t.HasAcceptedTerms).IsRequired();
            Property(t => t.HasEmailVerified).IsRequired();
            Property(t => t.CreatedTimestamp).IsRequired();
            Property(t => t.ModifiedTimestamp);
            //table
            ToTable("Users");

            //relationship
        }
    }
}