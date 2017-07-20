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
            var result = _investmentService.GetMatchingBonusResult(User.Identity.GetUserId<int>());
            return View();
        }

        //public ActionResult Create()
        //{
        //    //Find all downline users
        //    MatchingBonusListViewModels model = new MatchingBonusListViewModels
        //    {
                
        //    };
        //}

        [HttpPost]
        public ActionResult Create()
        {
            return View();
        }

        public JsonResult MatchingBonusList(int offset, int limit, string sort, string order)
        {
            var queryableList = _userService.GetReferralChildren(User.Identity.GetUserId<int>());
            var allRowCount = queryableList.Count();

            queryableList = queryableList.PaginateList("Generation", sort, order, offset, limit);

            var rowsResult = queryableList.ToList()
                .Select(x => new MatchingBonusListViewModels()
                {
                    //MatchingBonusId = 
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