using System;
using System.Collections;
using System.Collections.Generic;

namespace FIX.Core.Data
{
    public class Role : BaseEntity
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
