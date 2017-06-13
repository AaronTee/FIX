using System;
using System.Collections;
using System.Collections.Generic;

namespace FIX.Core.Data
{
    public class Bank : BaseEntity
    {
        public int BankId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<UserBankAccount> UserBankAccount { get; set; }
    }
}
