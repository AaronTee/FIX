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
    
    public partial class UserPackageDetail
    {
        public int UserPackageId { get; set; }
        public System.DateTime ReturnDate { get; set; }
        public decimal ReturnAmount { get; set; }
        public int StatusId { get; set; }
    
        public virtual Status Status { get; set; }
        public virtual UserPackage UserPackage { get; set; }
    }
}