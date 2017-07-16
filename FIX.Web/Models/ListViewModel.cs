using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FIX.Web.Models
{
    public class ListViewModel
    {
        public List<ActionLink> ActionLinks { get; set; }
    }

    public class ActionLink
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public class ActionTag
    {
        public string Action { get; set; }
        public string Name { get; set; }
    }
}