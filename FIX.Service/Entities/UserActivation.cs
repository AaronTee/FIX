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
    
    public partial class UserActivation
    {
        public int UserId { get; set; }
        public System.Guid ActivationCode { get; set; }
        public int StatusId { get; set; }
        public System.DateTime ExpiredTimestamp { get; set; }
    
        public virtual User User { get; set; }
        public virtual Status Status { get; set; }
    }
}
