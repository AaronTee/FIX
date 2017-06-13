using System.Data.Entity.ModelConfiguration;
using FIX.Core.Data;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIX.Data.Mapping
{
    public class BankMap : EntityTypeConfiguration<Bank>
    {
        public BankMap()
        {
            //key
            HasKey(t => t.BankId).Property(p => p.BankId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //properties
            Property(t => t.Name).IsRequired();
            //table
            ToTable("Bank");
        }
    }
}