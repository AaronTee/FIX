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
    
    public partial class UserPackage
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UserPackage()
        {
            this.UserPackageDetail = new HashSet<UserPackageDetail>();
        }
    
        public int UserPackageId { get; set; }
        public int UserId { get; set; }
        public int PackageId { get; set; }
        public decimal TotalAmount { get; set; }
        public System.DateTime Date { get; set; }
        public int StatusId { get; set; }
        public System.DateTime CreatedTimestamp { get; set; }
        public Nullable<System.DateTime> ModifiedTimestamp { get; set; }
    
        public virtual Package Package { get; set; }
        public virtual Status Status { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserPackageDetail> UserPackageDetail { get; set; }
        public virtual User User { get; set; }
    }
}
