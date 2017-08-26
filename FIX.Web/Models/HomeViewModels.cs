using System;
using System.ComponentModel.DataAnnotations;

namespace FIX.Web.Models
{
    public class HomeViewModels
    {
        public UserViewModel userModel { get; set; }

        public decimal WalletBalance { get; set; }
        public decimal BonusAmount { get; set; }
        public int ActivePackagesCount { get; set; }
        public string AnnouncementHtml { get; set; }
    }
}