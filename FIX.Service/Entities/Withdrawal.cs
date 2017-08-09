//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class Withdrawal
    {
        public int WithdrawalId { get; set; }
        public System.Guid WalletId { get; set; }
        public string ReferenceNo { get; set; }
        public string NotifyEmail { get; set; }
        public decimal WithdrawAmount { get; set; }
        public int BankId { get; set; }
        public string BankAccountName { get; set; }
        public string BankAccountNo { get; set; }
        public string BankBranch { get; set; }
        public int StatusId { get; set; }
        public System.DateTime CreatedTimestamp { get; set; }
        public Nullable<System.DateTime> ApprovedTimestamp { get; set; }
    
        public virtual Bank Bank { get; set; }
        public virtual Status Status { get; set; }
        public virtual UserWallet UserWallet { get; set; }
    }
}
