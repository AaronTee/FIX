using FIX.Service.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FIX.Web.Models
{
    public class InvestmentViewModel
    {

        public InvestmentCreateModel createModel { get; set; }
    }

    public class InvestmentCreateModel
    {

        public IEnumerable<Package> PackageList { get; set; }

        [Display(Name = "Invest Amount")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.00}")]
        [Range(typeof(Decimal), "500", "10000000", ErrorMessage = "{0} must be a decimal or number between {1} and {2}.")]
        [RegularExpression(@"^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Amount must be non-negative and two decimal places.")]
        public decimal Amount { get; set; }

        [Display(Name = "Entitled Package")]
        public string PackageName { get; set; }

        [Display(Name = "Rate")]
        public string Rate { get; set; }
    }

}