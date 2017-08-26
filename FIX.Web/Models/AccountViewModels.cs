using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace FIX.Web.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [Display(Name = "Password")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])\\S{8,}$", ErrorMessage = "Password must be a minimum of 8 characters and contain at least one capital letter.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Password is not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Token { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class EmailViewModel
    {
        [Display(Name = "Email")]
        public string Email { get; set; }
        public string EmailDomain { get; set; }
    }

    public class SetupViewModel
    {
        public SetupViewModel()
        {
            PackageList = new List<PackageInfo>();
        }

        public struct PackageInfo
        {
            public string PackageDescription { get; set; }
            public string PackageThreshold { get; set; }
            public string ReturnRate { get; set; }
            public string styleClass { get; set; }
        }

        public List<PackageInfo> PackageList { get; set; }

        [Required]
        [Display(Name = "Receipt Image")]
        public HttpPostedFileBase ReceiptFile { get; set; }

        [Required]
        [Display(Name = "Bank")]
        public string Bank { get; set; }

        [Display(Name = "Receipt Reference No.")]
        public string ReferenceNo { get; set; }

        [Required]
        [Display(Name = "Receipt Date")]
        public string Date { get; set; }

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
