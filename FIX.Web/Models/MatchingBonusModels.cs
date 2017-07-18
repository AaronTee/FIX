using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FIX.Web.Models
{
    public class MatchingBonusListViewModels{
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Package { get; set; }
        public int Generation { get; set; }
        public decimal? BonusAmount { get; set; }
    }

}