using FIX.Service;
using FIX.Web.Extensions;
using FIX.Web.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using static FIX.Service.DBConstant;

namespace FIX.Web.Controllers
{
    [Authorize]
    public class ReturnManagerController : BaseController
    {
        private IInvestmentService _investmentService;
        private IFinancialService _financialService;
        private IDocService _docService;

        public ReturnManagerController(IInvestmentService investmentService, IFinancialService financialService, IDocService docService) : base()
        {
            _investmentService = investmentService;
            _financialService = financialService;
            _docService = docService;
        }

        // GET: Investor
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetPendingReturnList(int offset, int limit, string search, string sort, string order)
        {
            var queryableList = _investmentService.GetAllPendingReturn();
            var allRowCount = queryableList.Count();

            if (search.IsNotNullOrEmpty())
            {
                queryableList = queryableList.Where(x => x.Username.ToString().Contains(search));
            }

            queryableList = queryableList.PaginateList(x => x.Date, sort, order, offset, limit);

            var rowsResult = queryableList.ToList().Select(x => new
            {
                UPDId = x.UPDId,
                Username = x.Username,
                Date = x.Date.ToUserLocalDate(User.Identity.GetUserTimeZone()),
                Package = x.Package,
                TotalInvest = x.TotalInvest,
                Rate = x.Rate,
                Amount = x.Amount,
                Status = x.Status,
                ActionText = new List<ActionTag>
                {
                    new ActionTag
                    {
                        Action = "approve",
                        Name = "Approve"
                    },
                    new ActionTag
                    {
                        Action = "void",
                        Name = "Void"
                    }
                }
            });

            var model = new
            {
                total = allRowCount,
                rows = rowsResult
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        //Return update row information to bootstrap table.
        public JsonResult ApproveReturn(int UPDId)
        {
            try
            {
                var upd = _investmentService.GetUserPackageDetail(UPDId);
                upd.StatusId = (int)EStatus.Approved;

                //Get DocCode
                var docCode = _docService.GetNextDocumentNumber(DBCDocSequence.EDocSequenceId.Interest_Return);

                //Add to wallet
                _financialService.TransactWalletCredit(EOperator.ADD, ETransactionType.Interest_Return, upd.Amount, docCode, upd.UserPackage.User.UserWallet.First().WalletId);

                //Check if package subscription ended, update status.
                var packagesDetail = _investmentService.GetAllUserPackageDetail(upd.UserPackageId);
                if (packagesDetail.Where(x => x.UserPackageDetailId != UPDId).Any(x => x.StatusId != (int)EStatus.Pending))
                {
                    var package = _investmentService.GetUserPackage(upd.UserPackageId);
                    package.StatusId = (int)EStatus.Deactivated;
                }

                _financialService.SaveChange(User.Identity.GetUserId<int>());

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                Log.Error(ex.Message, ex);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        //Return update row information to bootstrap table.
        public JsonResult VoidReturn(int UPDId)
        {
            var upd = _investmentService.GetUserPackageDetail(UPDId);
            upd.StatusId = (int)EStatus.Void;

            _investmentService.SaveChange(User.Identity.GetUserId<int>());
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}