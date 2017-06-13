using System;

namespace FIX.Core
{
    public abstract class BaseEntity
    {
        public DateTime CreatedTimestamp { get; set; }
        public DateTime? ModifiedTimestamp { get; set; }
    }
}