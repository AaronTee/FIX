using FIX.Service;
using FIX.Service.Entities;
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
    public class MatchingBonusController : Controller
    {
        private IUserService _userService;

        // GET: MatchingBonus
        public ActionResult Index(IUserService userService)
        {
            return View();
        }

        public ActionResult Create()
        {
            //Find all downline users
            MatchingBonusListViewModels model = new MatchingBonusListViewModels
            {
                UserId = 
            };
        }

        [HttpPost]
        public ActionResult Create()
        {

        }

        public JsonResult MatchingBonusList(int offset, int limit, string sort, string order)
        {
            var queryableList = _userService.GetReferralChildren(User.Identity.GetUserId<int>());
            var allRowCount = queryableList.Count();

            queryableList = queryableList.PaginateList("Generation", sort, order, offset, limit);

            var rowsResult = queryableList.ToList()
                .Select(x => new MatchingBonusListViewModels()
                {
                    MatchingBonusId = 
                });

            var model = new
            {
                total = allRowCount,
                rows = rowsResult
            };


            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
    }
}