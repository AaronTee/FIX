using FIX.Service;
using FIX.Web.Extensions;
using FIX.Web.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FIX.Web.Controllers
{
    [Authorize]
    public class WalletController : BaseController
    {
        private IFinancialService _financialService;

        public WalletController(IFinancialService financialService)
        {
            _financialService = financialService;
        }

        // GET: Wallet
        public ActionResult Index()
        {
            var walletId = _financialService.GetUserWallet(User.Identity.GetUserId<int>()).WalletId;
            WalletViewModel model = new WalletViewModel
            {
                CreditBalance = (_financialService.GetUserWalletBalance(walletId) ?? decimal.Zero).toCurrencyFormat(),
                MonthlyReturnAmount = (_financialService.GetReturnInterestReceivedAmount(walletId) ?? decimal.Zero).toCurrencyFormat(),
                MatchingBonusAmount = (_financialService.GetMatchingBonusReceivedAmount(walletId) ?? decimal.Zero).toCurrencyFormat(),
                WithdrawalAmount =( _financialService.GetWithdrawalAmount(walletId) ?? decimal.Zero).toCurrencyFormat()
            };

            return View(model);
        }

        public JsonResult Transaction(int offset, int limit, string sort, string order)
        {
            var walletId = _financialService.GetUserWallet(User.Identity.GetUserId<int>()).WalletId;
            var tz = User.Identity.GetUserTimeZone();
            var rawQueryableList = _financialService.GetWalletTransaction(walletId).Select(x => new
            {
                WalletTransactionId = x.WalletTransactionId,
                TransactionDate = x.TransactionDate,
                ReferenceNo = x.ReferenceNo,
                TransactionType = x.TransactionType,
                Debit = x.Debit,
                Credit = x.Credit
            });

            rawQueryableList = rawQueryableList.PaginateList(x => x.TransactionDate, "TransactionDate", "desc", offset, limit);

            var rowsResult = new List<WalletListViewModels>();
            decimal totalDebit = decimal.Zero;
            decimal totalCredit = decimal.Zero;

            foreach (var item in rawQueryableList)
            {
                totalDebit += item.Debit ?? decimal.Zero;
                totalCredit += item.Credit ?? decimal.Zero;

                var walletTransaction = new WalletListViewModels()
                {
                    WalletTransactionId = item.WalletTransactionId,
                    TransactionDate = item.TransactionDate.ToUserLocalDate(tz),
                    ReferenceNo = item.ReferenceNo + " - " + item.TransactionType,
                    Debit = (item.Debit.HasValue) ? item.Debit.Value.toCurrencyFormat() : null,
                    Credit = (item.Credit.HasValue) ? item.Credit.Value.toCurrencyFormat() : null,
                    Balance = (totalCredit - totalDebit).toCurrencyFormat()
                };

                rowsResult.Add(walletTransaction);
            }

            //If last page
            var isLastPage = ((offset + rowsResult.Count + 1) > rawQueryableList.Count());
            if (isLastPage)
            {
                //Footer
                rowsResult.Add(new WalletListViewModels()
                {
                    Debit = totalDebit.toCurrencyFormat(),
                    Credit = totalCredit.toCurrencyFormat(),
                    Balance = rowsResult.Last().Balance
                });
            }

            var model = new
            {
                total = rawQueryableList.Count(),
                rows = rowsResult
            };

            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}