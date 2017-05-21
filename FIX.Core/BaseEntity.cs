using System;

namespace FIX.Core
{
    public abstract class BaseEntity
    {
        public Int64 ID { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public DateTime ModifiedTimestamp { get; set; }
    }
}