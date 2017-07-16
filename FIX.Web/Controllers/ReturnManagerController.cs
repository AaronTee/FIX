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

        public ReturnManagerController(IInvestmentService investmentService) : base()
        {
            _investmentService = investmentService;
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

            queryableList = queryableList.PaginateList("Date", sort, order, offset, limit);

            var rowsResult = queryableList.ToList().Select(x => new
            {
                UPDId = x.UPDId,
                Username = x.Username,
                Date = x.Date.ToUserLocalDate(User.Identity.GetUserTimeZone()),
                Package = x.Package,
                TotalInvest = x.TotalInvest,
                Rate = x.Rate * 100,
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
            var upd = _investmentService.GetUserPackageDetail(UPDId);
            upd.StatusId = (int)EStatus.Approved;

            _investmentService.SaveChange(User.Identity.GetUserId<int>());

            return Json(true, JsonRequestBehavior.AllowGet);
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