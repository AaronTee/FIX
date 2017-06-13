using System.Data.Entity.ModelConfiguration;
using FIX.Core.Data;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIX.Data.Mapping
{
    public class UserBankAccountMap : EntityTypeConfiguration<UserBankAccount>
    {
        public UserBankAccountMap()
        {
            HasKey(t => new {
                t.UserId,
                t.BankId
            });
            //properties
            Property(t => t.BankAccountHolder).IsRequired();
            Property(t => t.BankAccountNo).IsRequired();
            Property(t => t.BankBranch).IsRequired();
            Property(t => t.IsPrimary).IsRequired();
            Property(t => t.CreatedTimestamp).IsRequired();
            Property(t => t.ModifiedTimestamp);
            //table
            ToTable("UserBankAccount");

            HasRequired(t => t.Bank).WithMany().HasForeignKey(t => t.BankId);
            HasRequired(t => t.User).WithMany().HasForeignKey(t => t.UserId);
        }
    }
}