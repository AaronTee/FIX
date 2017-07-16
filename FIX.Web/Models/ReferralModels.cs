using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FIX.Web.Models
{
    public class ReferralTreeNode
    {
        public int id { get; set; }
        public string text { get; set; }
        public object children { get; set; }
        public object li_attr { get; set; }
        public object icon { get; set; }
        //public ActionLink ProfileLink { get; set; }
    }

    public class ReferralListViewModels
    {
        public ReferralListViewModels()
        {
            TreeMaxLevel = 3;
        }

        public int TreeMaxLevel { get; set; }

        [Display(Name = "Search User")]
        public string Username { get; set; }
    }
}