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
    
    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            this.UserBankAccount = new HashSet<UserBankAccount>();
        }
    
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string IP { get; set; }
        public bool HasAcceptedTerms { get; set; }
        public bool HasEmailVerified { get; set; }
        public System.DateTime CreatedTimestamp { get; set; }
        public Nullable<System.DateTime> ModifiedTimestamp { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserBankAccount> UserBankAccount { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }
}
