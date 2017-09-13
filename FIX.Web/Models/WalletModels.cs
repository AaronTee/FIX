using FIX.Service.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FIX.Web.Models
{
    public class WalletViewModel
    {
        public string CreditBalance { get; set; }
        public string MatchingBonusAmount { get; set; }
        public string MonthlyReturnAmount { get; set; }
        public string WithdrawalAmount { get; set; }
    }

    public class WalletListViewModels
    {
        public int WalletTransactionId { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public string ReferenceNo { get; set; }
        public string Debit { get; set; }
        public string Credit { get; set; }
        public string Balance { get; set; }
    }

    public class WithdrawalViewModels
    {
        public WithdrawalViewModels()
        {
            UsePrimary = true;
        }

        [Display(Name = "E-Wallet Balance")]
        public string CreditBalance { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string NotifyEmail { get; set; }

        [Required]
        [Display(Name = "Withdraw Amount")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.00}")]
        [Range(typeof(Decimal), "10", "10000", ErrorMessage = "{0} must be a decimal or number between {1} and {2}.")]
        [RegularExpression(@"^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Amount must be non-negative and two decimal places.")]
        public decimal WithdrawAmount { get; set; }

        [Required]
        [Display(Name = "Bank Account No.")]
        public string BankAccountNo { get; set; }

        [Required]
        [Display(Name = "Bank")]
        public string Bank { get; set; }

        [Required]
        [Display(Name = "Bank Account Holder")]
        public string BankAccountHolder { get; set; }

        [Required]
        [Display(Name = "Bank Branch")]
        public string BankBranch { get; set; }

        public bool UsePrimary { get; set; }

        public SelectList BankDDL { get; internal set; }

    }
}