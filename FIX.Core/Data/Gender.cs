using System;
using System.Collections;
using System.Collections.Generic;

namespace FIX.Core.Data
{
    public class Gender : BaseEntity
    {
        public int GenderId { get; set; }
        public string Description { get; set; }
    }
}
