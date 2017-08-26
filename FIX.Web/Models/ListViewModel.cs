using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FIX.Web.Models
{
    public class ActionsLink
    {
        public bool checkBoxKey { get; set; }
        public List<ActionTag> ActionTags { get; set; }
        public List<ActionLink> ActionLinks { get; set; }
    }

    public class ActionLink
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string ClassName { get; set; }
    }

    public class ActionTag
    {
        public string Action { get; set; }
        public string Name { get; set; }
    }
}