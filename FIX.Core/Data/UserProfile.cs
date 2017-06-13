using System;
using System.Collections;

namespace FIX.Core.Data
{
    public class UserProfile : BaseEntity
    {
        public int UserProfileId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string PhoneNo { get; set; }
        public int Gender { get; set; }
        public virtual User User { get; set; }
    }
}