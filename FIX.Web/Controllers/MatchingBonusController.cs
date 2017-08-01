using FIX.Service;
using FIX.Service.Entities;
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
    public class MatchingBonusController : BaseController
    {
        private IUserService _userService;
        private IInvestmentService _investmentService;

        // GET: MatchingBonus
        public MatchingBonusController(IUserService userService, IInvestmentService investmentService)
        {
            _userService = userService;
            _investmentService = investmentService;
        }

        public ActionResult Index()
        {
            //Show matching bonus of this month by default.
            MatchingBonusSearchViewModels model = new MatchingBonusSearchViewModels
            {
                Date = DateTime.UtcNow.ConvertToDateYearMonthString()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Create()
        {
            return View();
        }

        public JsonResult MatchingBonusList(int offset, int limit, string sort, string order, string date)
        {
            List<MatchingBonus> queryableList = _investmentService.GetMatchingBonusList(User.Identity.GetUserId<int>()).ToList();

            if (!date.IsNullOrEmpty())
            {
                DateTime _date;
                DateTime.TryParseExact(date, DBCDateFormat.MMMyyyy, CultureInfo.CurrentCulture, DateTimeStyles.None, out _date);
                if (date != null)
                {
                    queryableList = queryableList.Where(x => x.ReturnDate >= _date && x.ReturnDate < _date.AddMonths(1)).ToList();
                }
            }

            decimal totalAmount = 0;
            totalAmount = queryableList.Sum(x => x.BonusAmount);

            //sorting
            var allRowCount = queryableList.Count();
            var sortedQueryableList = queryableList.PaginateList(sort, order, offset, limit, x=>x.Generation);

            var rowsResult = new List<MatchingBonusListViewModels>();

            var pos = offset + 1;
            
            foreach (var item in sortedQueryableList.ToList())
            {
                var matchingBonus = new MatchingBonusListViewModels()
                {
                    Pos = pos++.ToString(),
                    MatchingBonusId = item.MatchingBonusId,
                    Date = item.ReturnDate.ConvertToDateString(),
                    Username = item.Referral.Username,
                    Package = item.UserPackage.Package.Description,
                    Generation = item.Generation.ToString(),
                    BonusAmount = item.BonusAmount.ToString()
                };
                rowsResult.Add(matchingBonus);
            }

            //If last page
            var isLastPage = ((offset + rowsResult.Count + 1) > queryableList.Count());
            if (isLastPage)
            {
                //Footer
                rowsResult.Add(new MatchingBonusListViewModels()
                {
                    Pos = "Total: ",
                    BonusAmount = totalAmount.ToString()
                });
            }

            var model = new
            {
                total = allRowCount,
                rows = rowsResult
            };

            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}