using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FIX.Web.Models
{
    public class MatchingBonusSearchViewModels
    {
        public string Date { get; set; }
    }

    public class MatchingBonusListViewModels{
        public string Pos { get; set; }
        public int MatchingBonusId { get; set; }
        public string Date { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Package { get; set; }
        public string Generation { get; set; }
        public string BonusAmount { get; set; }
    }

}