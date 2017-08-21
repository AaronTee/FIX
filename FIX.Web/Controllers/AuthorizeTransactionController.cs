using FIX.Service;
using FIX.Web.Extensions;
using FIX.Web.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static FIX.Service.DBConstant;

namespace FIX.Web.Controllers
{
    [Authorize(Roles = DBCRole.Admin)]
    public class AuthorizeTransactionController : BaseController
    {
        private IUserService _userService;
        private IFinancialService _financialService;

        public struct AuthorizeTransactionPendingListSearchTerm
        {
            public int? UserId { get; set; }
            public string ReferenceNo { get; set; }
            public string TransactionType { get; set; }
            public string DateFrom { get; set; }
            public string DateTo { get; set; }
            public int? StatusId { get; set; }
        }

        public AuthorizeTransactionController(IUserService userService, IFinancialService financialService)
        {
            _userService = userService;
            _financialService = financialService;
        }

        // GET: AuthorizeTransaction
        public ActionResult Index()
        {
            AuthorizeTransactionViewModels model = new AuthorizeTransactionViewModels();
            
            return View(model);
        }

        //Support only one wallet for now. Search by userid instead.
        public JsonResult PendingList(int offset, int limit, AuthorizeTransactionPendingListSearchTerm searchField, string sort, string order)
        {
            if (!searchField.UserId.HasValue) return Json(new { }, JsonRequestBehavior.AllowGet);

            var queryableList = _financialService.GetAllWalletPendingPreauthorizeTransactionList(searchField.UserId.Value);
            var allRowCount = queryableList.Count();

            //Filter reference number.
            if (searchField.ReferenceNo.IsNotNullOrEmpty())
            {
                queryableList = queryableList.Where(x => x.ReferenceNo.Contains(searchField.ReferenceNo));
            }

            //Filter transaction type.
            if (searchField.TransactionType.IsNotNullOrEmpty())
            {
                queryableList = queryableList.Where(x => x.TransactionType == searchField.TransactionType);
            }

            if (!searchField.DateFrom.IsNullOrEmpty())
            {
                DateTime date;
                DateTime.TryParseExact(searchField.DateFrom, DBCDateFormat.ddMMMyyyy, CultureInfo.CurrentCulture, DateTimeStyles.None, out date);
                if (date != null)
                {
                    queryableList = queryableList.Where(x => x.TransactionDate >= date);
                }
            }

            if (!searchField.DateTo.IsNullOrEmpty())
            {
                DateTime date;
                DateTime.TryParseExact(searchField.DateTo, DBCDateFormat.ddMMMyyyy, CultureInfo.CurrentCulture, DateTimeStyles.None, out date);

                if (date != null)
                {
                    queryableList = queryableList.Where(x => x.TransactionDate <= date);
                }
            }

            if (searchField.StatusId.HasValue)
            {
                queryableList = queryableList.Where(x => x.StatusId == searchField.StatusId.Value);
            }

            queryableList = queryableList.PaginateList(x => x.TransactionDate, sort, order, offset, limit).OrderByDescending(x => x.TransactionDate);

            var rowsResult = queryableList.ToList().Select(x => new PreauthListViewModels
            {
                PreauthId = x.PreauthId.ToString(),
                TransactionDate = x.TransactionDate.ToUserLocalDateTime(User.Identity.GetUserTimeZone()),
                TransactionType = x.TransactionType,
                ReferenceNo = x.ReferenceNo,
                Debit = x.Debit.ToString(),
                Credit = x.Credit.ToString(),
                Status = x.Status.Description,
                ActionTags = new Func<List<ActionTag>>(() =>
                {
                    List<ActionTag> actions = new List<ActionTag>();
                    if (x.StatusId == (int)EStatus.Pending)
                    {
                        actions.Add(new ActionTag
                        {
                            Action = "authorize",
                            Name = "Authorize"
                        });

                        actions.Add(new ActionTag
                        {
                            Action = "void",
                            Name = "Void"
                        });
                    }
                    return actions;
                })()
            });

            var model = new
            {
                total = allRowCount,
                rows = rowsResult
            };
            return Json(model, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Authorize(int? PreauthId)
        {
            EJState result = EJState.Unknown;
            try
            {   
                var preauthTransaction = _financialService.GetPreauthTransaction(PreauthId);
                _financialService.TransactWalletCredit(EOperator.DEDUCT, ETransactionType.Withdrawal, preauthTransaction.Debit ?? decimal.Zero, preauthTransaction.ReferenceNo, preauthTransaction.WalletId);
                preauthTransaction.StatusId = (int)EStatus.Approved;

                _financialService.SaveChange(User.Identity.GetUserId<int>());
                result = EJState.Success;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                result = EJState.Failed;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Void(int? PreauthId)
        {
            EJState result = EJState.Unknown;
            try
            {
                var preauthTransaction = _financialService.GetPreauthTransaction(PreauthId);
                preauthTransaction.StatusId = (int)EStatus.Void;

                //TODO: Adjust back to user wallet.

                _financialService.SaveChange(User.Identity.GetUserId<int>());
                result = EJState.Success;

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                result = EJState.Failed;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}