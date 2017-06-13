using FIX.Core.Data;
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
            Roles = new List<Role>();
        }

        public int UserId { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string Address { get; set; }

        [Display(Name = "Username")]
        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        [Display(Name = "Roles")]
        public IEnumerable<Role> Roles { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedTimestamp { get; set; }
    }

    public class UserCreateEditViewModel
    {
        public UserCreateEditViewModel()
        {
            CountryDDL = new List<SelectListItem>();
            RoleDDL = new List<SelectListItem>();
            GenderDDL = new List<SelectListItem>();
        }

        [Required]
        [Display(Name = "Member ID")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Password not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public int Gender { get; set; }

        [Required]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Required]
        [Display(Name = "Role")]
        public List<int> Roles { get; set; }

        [Required]
        [Display(Name = "Phone No.")]
        public string PhoneNo { get; set; }

        [Required]
        [Display(Name = "Bank Account No.")]
        public string BankAccountNo { get; set; }

        [Required]
        [Display(Name = "Bank Account Holder")]
        public string BankAccountHolder { get; set; }

        [Required]
        [Display(Name = "Bank Branch")]
        public string BankBranch { get; set; }

        public IEnumerable<SelectListItem> CountryDDL { get; set; }
        public IEnumerable<SelectListItem> RoleDDL { get; set; }
        public IEnumerable<SelectListItem> GenderDDL { get; set; }


        public UserRole UserRole { get; set; }
    }

    public class UserRole
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string Admin = "Admin";
        public const string User = "User";
    }
}