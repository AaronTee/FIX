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
    
        public virtual DbSet<Bank> Bank { get; set; }
        public virtual DbSet<Package> Package { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<UserBankAccount> UserBankAccount { get; set; }
        public virtual DbSet<UserActivation> UserActivation { get; set; }
        public virtual DbSet<UserPackage> UserPackage { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserPackageDetail> UserPackageDetail { get; set; }
        public virtual DbSet<vwPendingReturnInvestor> vwPendingReturnInvestor { get; set; }
        public virtual DbSet<vwPendingReturnInvestor_Test> vwPendingReturnInvestor_Test { get; set; }
        public virtual DbSet<database_firewall_rules> database_firewall_rules { get; set; }
        public virtual DbSet<UserProfile> UserProfile { get; set; }
        public virtual DbSet<AuditLog> AuditLog { get; set; }
        public virtual DbSet<DailyTrading> DailyTrading { get; set; }
    }
}
