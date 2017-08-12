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
    
    public partial class Preauth
    {
        public int PreauthId { get; set; }
        public System.Guid WalletId { get; set; }
        public string ReferenceNo { get; set; }
        public System.DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public int StatusId { get; set; }
        public Nullable<System.DateTime> AuthorizedTimestamp { get; set; }
        public string AuthorizedBy { get; set; }
    
        public virtual Status Status { get; set; }
        public virtual UserWallet UserWallet { get; set; }
    }
}