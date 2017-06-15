using System;
using System.Collections;
using System.Collections.Generic;

namespace FIX.Core.Data
{
    public class UserBankAccount : BaseEntity
    {
        public int UserBankAccountId { get; set; }
        public string BankAccountNo { get; set; }
        public string BankAccountHolder { get; set; }
        public string BankBranch { get; set; }
        public bool IsPrimary { get; set; }

        public virtual Bank Bank { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }
}
