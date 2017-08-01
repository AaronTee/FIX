﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FIX.Service.Entities
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class FIXEntities : DbContext
    {
        public FIXEntities()
            : base("name=FIXEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AuditLog> AuditLog { get; set; }
        public virtual DbSet<Bank> Bank { get; set; }
        public virtual DbSet<DailyTrading> DailyTrading { get; set; }
        public virtual DbSet<Package> Package { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserActivation> UserActivation { get; set; }
        public virtual DbSet<UserBankAccount> UserBankAccount { get; set; }
        public virtual DbSet<UserPackage> UserPackage { get; set; }
        public virtual DbSet<UserPackageDetail> UserPackageDetail { get; set; }
        public virtual DbSet<UserProfile> UserProfile { get; set; }
        public virtual DbSet<UserWallet> UserWallet { get; set; }
        public virtual DbSet<WalletTransaction> WalletTransaction { get; set; }
        public virtual DbSet<vwPendingReturnInvestor> vwPendingReturnInvestor { get; set; }
        public virtual DbSet<vwPendingReturnInvestor_Test> vwPendingReturnInvestor_Test { get; set; }
        public virtual DbSet<database_firewall_rules> database_firewall_rules { get; set; }
        public virtual DbSet<MatchingBonus> MatchingBonus { get; set; }
    
        public virtual ObjectResult<spMatchingBonus_Result> spMatchingBonus(Nullable<int> level, string userId, string updId, Nullable<int> offsetMonth, Nullable<int> offsetDay, Nullable<double> tier1BonusPerc, Nullable<double> tier2BonusPerc, Nullable<double> tier3BonusPerc)
        {
            var levelParameter = level.HasValue ?
                new ObjectParameter("level", level) :
                new ObjectParameter("level", typeof(int));
    
            var userIdParameter = userId != null ?
                new ObjectParameter("userId", userId) :
                new ObjectParameter("userId", typeof(string));
    
            var updIdParameter = updId != null ?
                new ObjectParameter("updId", updId) :
                new ObjectParameter("updId", typeof(string));
    
            var offsetMonthParameter = offsetMonth.HasValue ?
                new ObjectParameter("offsetMonth", offsetMonth) :
                new ObjectParameter("offsetMonth", typeof(int));
    
            var offsetDayParameter = offsetDay.HasValue ?
                new ObjectParameter("offsetDay", offsetDay) :
                new ObjectParameter("offsetDay", typeof(int));
    
            var tier1BonusPercParameter = tier1BonusPerc.HasValue ?
                new ObjectParameter("tier1BonusPerc", tier1BonusPerc) :
                new ObjectParameter("tier1BonusPerc", typeof(double));
    
            var tier2BonusPercParameter = tier2BonusPerc.HasValue ?
                new ObjectParameter("tier2BonusPerc", tier2BonusPerc) :
                new ObjectParameter("tier2BonusPerc", typeof(double));
    
            var tier3BonusPercParameter = tier3BonusPerc.HasValue ?
                new ObjectParameter("tier3BonusPerc", tier3BonusPerc) :
                new ObjectParameter("tier3BonusPerc", typeof(double));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spMatchingBonus_Result>("spMatchingBonus", levelParameter, userIdParameter, updIdParameter, offsetMonthParameter, offsetDayParameter, tier1BonusPercParameter, tier2BonusPercParameter, tier3BonusPercParameter);
        }
    
        public virtual ObjectResult<spMatchingBonusFormula_experimental_Result> spMatchingBonusFormula_experimental(Nullable<int> level, string userId, string updId, string userPackageId, Nullable<int> offsetMonth, Nullable<int> offsetDay, Nullable<double> tier1BonusPerc, Nullable<double> tier2BonusPerc, Nullable<double> tier3BonusPerc)
        {
            var levelParameter = level.HasValue ?
                new ObjectParameter("level", level) :
                new ObjectParameter("level", typeof(int));
    
            var userIdParameter = userId != null ?
                new ObjectParameter("userId", userId) :
                new ObjectParameter("userId", typeof(string));
    
            var updIdParameter = updId != null ?
                new ObjectParameter("updId", updId) :
                new ObjectParameter("updId", typeof(string));
    
            var userPackageIdParameter = userPackageId != null ?
                new ObjectParameter("UserPackageId", userPackageId) :
                new ObjectParameter("UserPackageId", typeof(string));
    
            var offsetMonthParameter = offsetMonth.HasValue ?
                new ObjectParameter("offsetMonth", offsetMonth) :
                new ObjectParameter("offsetMonth", typeof(int));
    
            var offsetDayParameter = offsetDay.HasValue ?
                new ObjectParameter("offsetDay", offsetDay) :
                new ObjectParameter("offsetDay", typeof(int));
    
            var tier1BonusPercParameter = tier1BonusPerc.HasValue ?
                new ObjectParameter("tier1BonusPerc", tier1BonusPerc) :
                new ObjectParameter("tier1BonusPerc", typeof(double));
    
            var tier2BonusPercParameter = tier2BonusPerc.HasValue ?
                new ObjectParameter("tier2BonusPerc", tier2BonusPerc) :
                new ObjectParameter("tier2BonusPerc", typeof(double));
    
            var tier3BonusPercParameter = tier3BonusPerc.HasValue ?
                new ObjectParameter("tier3BonusPerc", tier3BonusPerc) :
                new ObjectParameter("tier3BonusPerc", typeof(double));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spMatchingBonusFormula_experimental_Result>("spMatchingBonusFormula_experimental", levelParameter, userIdParameter, updIdParameter, userPackageIdParameter, offsetMonthParameter, offsetDayParameter, tier1BonusPercParameter, tier2BonusPercParameter, tier3BonusPercParameter);
        }
    }
}
