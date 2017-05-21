using System;

namespace FIX.Core.Data
{
    public class User : BaseEntity
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string IP { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }
}
