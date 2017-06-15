﻿using System;
using System.Collections;
using System.Collections.Generic;

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

        public virtual Gender Gender { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<UserBankAccount> UserBankAccount { get; set; }
    }
}