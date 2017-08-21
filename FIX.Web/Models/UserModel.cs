using FIX.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FIX.Web.Models
{

    public class UserViewModel
    {
        public UserViewModel()
        {
            RoleDDL = new SelectList(new List<SelectListItem>());
            ReferralDDL = new SelectList(new List<SelectListItem>());
            CountryDDL = new SelectList(
                new List<SelectListItem>()
                {
                    new SelectListItem
                    {
                        Text = "Malaysia",
                        Value = "MY"
                    }
                }, "Value", "Text", Country);

            GenderDDL = new SelectList(new List<SelectListItem>()
            {
                new SelectListItem() { Text = "Male", Value = DBConstant.DBCGender.Male },
                new SelectListItem() { Text = "Female", Value = DBConstant.DBCGender.Female },
                new SelectListItem() { Text = "Other", Value = DBConstant.DBCGender.Other }
            }, "Value", "Text", Gender);
        }

        public int Id { get; set; }

        [Required]
        [Display(Name = "Member ID")]
        [RegularExpression("^[a-zA-Z0-9]{5,16}$", ErrorMessage = "Please enter 5-16 alphanumeric character without any separator (no dashes, space and etc).")]
        [System.Web.Mvc.Remote("ValidateUsername", "User", HttpMethod = "POST", ErrorMessage = "Username has been taken.")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Password")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d]{8,32}$", ErrorMessage = "Password must be a minimum of 8 characters and contain at least letter, one capital letter and a number.")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Password not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Security Password")]
        [RegularExpression("^[0-9]{6,6}$", ErrorMessage = "Please enter exact 6 numeric digit.")]
        public string SecurityPassword { get; set; }

        [Required]
        [Display(Name = "Name")]
        [RegularExpression("^[a-zA-Z\\s]*$", ErrorMessage = "Please enter only letters.")]
        public string Name { get; set; }
        
        [Required]
        [Display(Name = "I/C Number")]
        [RegularExpression("^[a-zA-Z0-9]{1,}$", ErrorMessage = "Please enter alphanumeric character without any separator (no dashes, space and etc).")]
        public string ICNumber { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "State")]
        public string State { get; set; }

        [Display(Name = "Post Code")]
        public string PostCode { get; set; }

        [Required]
        [Display(Name = "Email")]
        [Remote("ValidateEmail", "User", HttpMethod = "POST", ErrorMessage = "Email already exists")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        public string GenderDescription { get; set; }

        [Display(Name = "Bank")]
        public string BankName { get; set; }

        [Required]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Display(Name = "Role")]
        public string RoleName { get; set; }

        [Required]
        [Display(Name = "Role")]
        public int RoleId { get; set; }

        [Required]
        [Display(Name = "Referral")]
        public int ReferralId { get; set; }

        [Display(Name = "Referral")]
        public string ReferralName { get; set; }

        [Required]
        [Display(Name = "Phone No.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Please enter numeric character only (No dashes [-]).")]
        public string PhoneNo { get; set; }

        [Required]
        [Display(Name = "Bank Account No.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Please enter numeric character only (No dashes [-]).")]
        public string BankAccountNo { get; set; }

        [Required]
        [Display(Name = "Bank")]
        public int BankId { get; set; }

        [Required]
        [Display(Name = "Bank Account Holder")]
        public string BankAccountHolder { get; set; }

        [Required]
        [Display(Name = "Bank Branch")]
        public string BankBranch { get; set; }

        [Display(Name = "Balance")]
        public string CreditBalance { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedTimestamp { get; set; }

        public bool hasEmailVerified { get; set; }
        public bool hasAcceptedTerms { get; set; }

        public SelectList CountryDDL { get; set; }
        public SelectList RoleDDL { get; set; }
        public SelectList GenderDDL { get; set; }
        public SelectList BankDDL { get; set; }
        public SelectList ReferralDDL { get; set; }
    }

    public class UserListViewModel : ActionsLink{
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public string Status { get; set; }
    }

    public class UserRole
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string Admin = "Admin";
        public const string User = "User";
    }

    public class TermsAndContitionsViewModel
    {
        public string Terms { get; set; }
        public bool HasAgreed { get; set; }
    }
}