namespace FIX.Service.Entities
{
    using System;
    using System.Collections.Generic;

    public partial class MatchingBonus
    {
        public virtual User Referral
        {
            get { return this.User; }
            set
            {
                this.User = value;
            }
        }

        //referral
        public virtual User UserEntity
        {
            get { return this.User1; }
            set
            {
                this.User1 = value;
            }
        }

    }
}
