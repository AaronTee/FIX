using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FIX.Web.Models
{
    public class ManageAccountViewModel
    {

        public ManagePasswordViewModels ManagePasswordVM { get; set; }
        public ManagePersonalDetailViewModels ManagePersonalDetailVM { get; set; }
        public ManageBankAccountViewModels ManageBankAccountVM { get; set; }

        public string Username { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        [Display(Name = "Account Status")]
        public string Status { get; set; }
        [Display(Name = "Referral")]
        public string ReferralName { get; set; }
        public string CreditBalance { get; set; }

        public SelectList CountryDDL { get; set; }
        public SelectList BankDDL { get; set; }
    }

    public class ManagePasswordViewModels
    {
        /* Password Information */
        [Required]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }

        [Required]
        [Display(Name = "New Password")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])\\S{8,}$", ErrorMessage = "Password must be a minimum of 8 characters and contain at least one capital letter.")]
        public string NewPasword { get; set; }

        [Required]
        [Display(Name = "Confirm New Password")]
        [System.Web.Mvc.Compare(nameof(NewPasword), ErrorMessage = "Password not match.")]
        public string ConfirmNewPassword { get; set; }
    }

    public class ManagePersonalDetailViewModels
    {
        /* Personal Information */
        [Required]
        [Display(Name = "Name")]
        [RegularExpression("^[a-zA-Z\\s]*$", ErrorMessage = "Please enter only letters.")]
        public string Name { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "State")]
        public string State { get; set; }

        [Display(Name = "Post Code")]
        public string PostCode { get; set; }

        [Required]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Required]
        [Display(Name = "Phone No.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Please enter numeric character only (No dashes [-]).")]
        public string PhoneNo { get; set; }
    }

    public class ManageBankAccountViewModels
    {
        /* Banking Information */
        [Required]
        [Display(Name = "Bank")]
        public int BankId { get; set; }

        [Required]
        [Display(Name = "Bank Account Holder")]
        public string BankAccountHolder { get; set; }

        [Required]
        [Display(Name = "Bank Account No.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Please enter numeric character only (No dashes [-]).")]
        public string BankAccountNo { get; set; }

        [Required]
        [Display(Name = "Bank Branch")]
        public string BankBranch { get; set; }
    }
}