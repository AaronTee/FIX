using FIX.Service;
using FIX.Service.Entities;
using FIX.Web.Extensions;
using FIX.Web.Models;
using Microsoft.AspNet.Identity;
using SyntrinoWeb.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static FIX.Service.DBConstant;

namespace FIX.Web.Controllers
{
    [Authorize]
    [IdentityAuthorize]
    public class WalletController : BaseController
    {
        private IFinancialService _financialService;
        private IUserService _userService;
        private IBankService _bankService;
        private IDocService _docService;

        public WalletController(IFinancialService financialService, IUserService userService, IBankService bankService, IDocService docService)
        {
            _financialService = financialService;
            _userService = userService;
            _bankService = bankService;
            _docService = docService;
        }

        // GET: Wallet
        public ActionResult Index()
        {
            var walletId = _financialService.GetUserWallet(User.Identity.GetUserId<int>()).WalletId;
            WalletViewModel model = new WalletViewModel
            {
                CreditBalance = (_financialService.GetUserWalletAvailableBalance(walletId) ?? decimal.Zero).toCurrencyFormat(),
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

            rawQueryableList = rawQueryableList.PaginateList(x => x.TransactionDate, "TransactionDate", "asc", offset, limit);

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

        //[Authorize(Roles = DBCRole.Admin)]
        //public JsonResult Transaction(int offset, int limit, string sort, string order, int? UserId)
        //{
        //    if (UserId.IsNullOrEmpty()) return Json(new { }, JsonRequestBehavior.AllowGet);

        //    var walletId = _financialService.GetUserWallet(UserId.Value).WalletId;
        //    var tz = User.Identity.GetUserTimeZone();
        //    var rawQueryableList = _financialService.GetWalletTransaction(walletId).Select(x => new
        //    {
        //        WalletTransactionId = x.WalletTransactionId,
        //        TransactionDate = x.TransactionDate,
        //        ReferenceNo = x.ReferenceNo,
        //        TransactionType = x.TransactionType,
        //        Debit = x.Debit,
        //        Credit = x.Credit
        //    });

        //    rawQueryableList = rawQueryableList.PaginateList(x => x.TransactionDate, "TransactionDate", "desc", offset, limit);

        //    var rowsResult = new List<WalletListViewModels>();
        //    decimal totalDebit = decimal.Zero;
        //    decimal totalCredit = decimal.Zero;

        //    foreach (var item in rawQueryableList)
        //    {
        //        totalDebit += item.Debit ?? decimal.Zero;
        //        totalCredit += item.Credit ?? decimal.Zero;

        //        var walletTransaction = new WalletListViewModels()
        //        {
        //            WalletTransactionId = item.WalletTransactionId,
        //            TransactionDate = item.TransactionDate.ToUserLocalDate(tz),
        //            ReferenceNo = item.ReferenceNo + " - " + item.TransactionType,
        //            Debit = (item.Debit.HasValue) ? item.Debit.Value.toCurrencyFormat() : null,
        //            Credit = (item.Credit.HasValue) ? item.Credit.Value.toCurrencyFormat() : null,
        //            Balance = (totalCredit - totalDebit).toCurrencyFormat()
        //        };

        //        rowsResult.Add(walletTransaction);
        //    }

        //    //If last page
        //    var isLastPage = ((offset + rowsResult.Count + 1) > rawQueryableList.Count());
        //    if (isLastPage)
        //    {
        //        //Footer
        //        rowsResult.Add(new WalletListViewModels()
        //        {
        //            Debit = totalDebit.toCurrencyFormat(),
        //            Credit = totalCredit.toCurrencyFormat(),
        //            Balance = rowsResult.Last().Balance
        //        });
        //    }

        //    var model = new
        //    {
        //        total = rawQueryableList.Count(),
        //        rows = rowsResult
        //    };

        //    return Json(model, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult Withdrawal()
        {
            //Read user primary bank
            var userBankAccount = _userService.GetAllUserBankAccount(User.Identity.GetUserId<int>()).FirstOrDefault();
            var userWallet = _financialService.GetUserWallet(userBankAccount.UserId);

            WithdrawalViewModels model = new WithdrawalViewModels();

            if (userBankAccount != null)
            {
                model = new WithdrawalViewModels
                {
                    BankAccountHolder = userBankAccount.BankAccountHolder,
                    BankAccountNo = userBankAccount.BankAccountNo,
                    BankBranch = userBankAccount.BankBranch,
                    BankId = userBankAccount.BankId,
                    CreditBalance = (_financialService.GetUserWalletAvailableBalance(userWallet.WalletId) ?? decimal.Zero).toCurrencyFormat(),
                    NotifyEmail = userBankAccount.User.Email,
                };
            }

            model.BankDDL = new SelectList(_bankService.GetAllBank().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.BankId.ToString()
            }), "Value", "Text", model.BankId);

            return View(model);
        }

        [HttpPost]
        public ActionResult Withdrawal(WithdrawalViewModels model, string saveState)
        {
            try
            {
                var user = _userService.GetUserBy(User.Identity.GetUserId<int>());
                var userWallet = _financialService.GetUserWallet(user.UserId);

                //Insufficient Balance handling
                if (userWallet.Balance < model.WithdrawAmount)
                {
                    Warning("You have insufficient E-Wallet Credit to withdraw.");

                    var defaultModel = (WithdrawalViewModels)((ViewResultBase)Withdrawal()).ViewData.Model;
                    model.BankDDL = defaultModel.BankDDL;
                    model.CreditBalance = defaultModel.CreditBalance;

                    return RedirectToAction("Withdrawal");
                }

                if (ModelState.IsValid)
                {
                    var docCode = _docService.GetNextDocumentNumber(DBConstant.DBCDocSequence.EDocSequenceId.Withdrawal);
                    //Insert withdrawal record
                    Withdrawal wd = new Service.Entities.Withdrawal
                    {
                        WalletId = userWallet.WalletId,
                        ReferenceNo = docCode,
                        NotifyEmail = model.NotifyEmail,
                        BankId = model.BankId,
                        StatusId = (int)EStatus.Pending,
                        BankAccountName = model.BankAccountHolder,
                        BankAccountNo = model.BankAccountNo,
                        BankBranch = model.BankBranch,
                        CreatedTimestamp = DateTime.UtcNow,
                        WithdrawAmount = model.WithdrawAmount,
                    };
                    _financialService.InsertWithdrawal(wd);

                    //Preauth
                    _financialService.PreauthorizeWalletCredit(EOperator.DEDUCT ,ETransactionType.Withdrawal, model.WithdrawAmount, docCode, userWallet.WalletId);

                    //Commit
                    _financialService.SaveChange(User.Identity.GetUserId<int>());
                    Success("Your request to withdraw total amount of $" + model.WithdrawAmount.toCurrencyFormat() + " has been submitted.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                Danger("Failed to submit withdrawal request. Please try again later. If error still persist please contact our customer support.");
            }

            return RedirectToAction("Index");
        }
    }
}