using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FIX.Web.Models
{

    public class DailyTradingEditViewModels : DailyTradingBaseViewModels { }

    public class DailyTradingCreateViewModels : DailyTradingBaseViewModels
    {
        [Required]
        [Display(Name = "Date")]
        public override string Date { get; set; }
        [Required]
        [Display(Name = "EURJPY")]
        public override decimal? EURJPY { get; set; }
        [Required]
        [Display(Name = "EURUSD")]
        public override decimal? EURUSD { get; set; }
        [Required]
        [Display(Name = "EURNZD")]
        public override decimal? EURNZD { get; set; }
        [Required]
        [Display(Name = "USDCAD")]
        public override decimal? USDCAD { get; set; }
        [Required]
        [Display(Name = "GBPUSD")]
        public override decimal? GBPUSD { get; set; }
        [Required]
        [Display(Name = "USDSGD")]
        public override decimal? USDSGD { get; set; }
    }

    public abstract class DailyTradingBaseViewModels
    {
        [Display(Name = "Date")]
        public virtual string Date { get; set; }
        [Display(Name = "EURJPY")]
        public virtual decimal? EURJPY { get; set; }
        [Display(Name = "EURUSD")]
        public virtual decimal? EURUSD { get; set; }
        [Display(Name = "EURNZD")]
        public virtual decimal? EURNZD { get; set; }
        [Display(Name = "USDCAD")]
        public virtual decimal? USDCAD { get; set; }
        [Display(Name = "GBPUSD")]
        public virtual decimal? GBPUSD { get; set; }
        [Display(Name = "USDSGD")]
        public virtual decimal? USDSGD { get; set; }
        [Display(Name = "Total Profit")]
        public virtual decimal? Profit { get; set; }
    }

    public class DailyTradingListViewModels : ListViewModel
    {
        public string Date { get; set; }

        public decimal? EURJPY { get; set; }

        public decimal? EURUSD { get; set; }

        public decimal? EURNZD { get; set; }

        public decimal? USDCAD { get; set; }

        public decimal? GBPUSD { get; set; }

        public decimal? USDSGD { get; set; }

        public decimal? Profit { get; set; }
    }


    public class DailyTradingSearchViewModels
    {
        [Display(Name = "Date From")]
        public string DateFrom { get; set; }

        [Display(Name = "Date To")]
        public string DateTo { get; set; }
    }
}