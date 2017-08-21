using FIX.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FIX.Web.Models
{
    public class AuthorizeTransactionViewModels
    {
        public AuthorizeTransactionViewModels()
        {
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Pending", Value = ((int)DBConstant.EStatus.Pending).ToString() });
            status.Add(new SelectListItem() { Text = "Approved", Value = ((int)DBConstant.EStatus.Approved).ToString() });
            status.Add(new SelectListItem() { Text = "Void", Value = ((int)DBConstant.EStatus.Void).ToString() });
            StatusDDL = new SelectList(status, "Value", "Text");

            List<SelectListItem> tddl = new List<SelectListItem>();
            tddl.Add(new SelectListItem() { Text = "Interest Return", Value = Enum.GetName(typeof(DBConstant.ETransactionType), DBConstant.ETransactionType.Interest_Return).Replace("_", " ") });
            tddl.Add(new SelectListItem() { Text = "Matching Bonus", Value = Enum.GetName(typeof(DBConstant.ETransactionType), DBConstant.ETransactionType.Matching_Bonus).Replace("_", " ") });
            tddl.Add(new SelectListItem() { Text = "Withdrawal", Value = Enum.GetName(typeof(DBConstant.ETransactionType), DBConstant.ETransactionType.Withdrawal).Replace("_", " ") });
            TransactionTypeDDL = new SelectList(tddl, "Value", "Text");
        }

        [Display(Name = "Reference No")]
        public string ReferenceNo { get; set; }
        [Display(Name = "Transaction Type")]
        public string TransactionType { get; set; }
        [Display(Name = "User")]
        public int UserId { get; set; }
        [Display(Name = "Date From")]
        public string DateFrom { get; set; }
        [Display(Name = "Date To")]
        public string DateTo { get; set; }
        [Display(Name = "Status")]
        public int StatusId { get; set; }

        public SelectList UserDDL { get; set; } 
        public SelectList StatusDDL { get; set; }
        public SelectList TransactionTypeDDL { get; set; }
    }

    public class PreauthListViewModels : ActionsLink
    {
        public string PreauthId { get; set; }
        public string TransactionDate { get; set; }
        public string ReferenceNo { get; set; }
        public string TransactionType { get; set; }
        public string Debit { get; set; }
        public string Credit { get; set; }
        public string Status { get; set; }
    }

}