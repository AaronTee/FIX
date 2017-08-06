using FIX.Service.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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
}