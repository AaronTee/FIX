using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FIX.Web.Models
{
    public class MatchingBonusSearchViewModels
    {
        public string Date { get; set; }
        public string UserId { get; set; }
        public SelectList UserDDL { get; set; }

    }

    public class MatchingBonusListViewModels : ActionsLink{
        public string Pos { get; set; }
        public string MatchingBonusId { get; set; }
        public string Date { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Package { get; set; }
        public string Generation { get; set; }
        public string BonusAmount { get; set; }
        public string Status { get; set; }
    }

}