using System;
using System.Collections.Generic;

namespace FIX.Core.Data
{
    public class User : BaseEntity
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string IP { get; set; }
        public bool HasAcceptedTerms { get; set; }
        public bool HasEmailVerified { get; set; }
        public virtual UserProfile UserProfile { get; set; }

    }
}
