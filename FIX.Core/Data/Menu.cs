using System;

namespace FIX.Core.Data
{
    public class Menu : BaseEntity
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
