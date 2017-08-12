using FIX.Web.Models;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using FIX.Web.Extensions;
using FIX.Service;
using System.Linq;
using FIX.Service.Entities;
using System;
using static FIX.Service.DBConstant;
using System.Security.Claims;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;
using System.Globalization;

namespace FIX.Web.Controllers
{
    [Authorize]
    public class DailyTradingController : BaseController
    {

        private IReportService _reportService;

        public DailyTradingController(IReportService reportService)
        {
            _reportService = reportService;
        }

        public ActionResult Index()
        {
            DailyTradingSearchViewModels model = new DailyTradingSearchViewModels();
            return View(model);
        }

        [Authorize(Roles = DBCRole.Admin)]
        public ActionResult Create()
        {
            DailyTradingBaseViewModels model = new DailyTradingCreateViewModels();
            return View(model);
        }

        [Authorize(Roles = DBCRole.Admin)]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(DailyTradingCreateViewModels model)
        {
            try
            {
                if (!model.Date.IsValidStringDate())
                {
                    ModelState.AddModelError(model.Date, "Invalid date input.");
                }

                if (ModelState.IsValid)
                {
                    var totalProfit = model.EURJPY + model.EURNZD + model.EURUSD
                        + model.GBPUSD + model.USDCAD + model.USDSGD;

                    DailyTrading newDailyTrading = new DailyTrading
                    {
                        Date = model.Date.ConvertToDate().Value,
                        EURJPY = model.EURJPY,
                        EURNZD = model.EURNZD,
                        EURUSD = model.EURUSD,
                        GBPUSD = model.GBPUSD,
                        USDCAD = model.USDCAD,
                        USDSGD = model.USDSGD,
                        Profit = totalProfit,
                        CreatedTimestamp = DateTime.UtcNow,
                    };

                    _reportService.InsertDailyTrading(newDailyTrading);
                }

                if (!_reportService.SaveChanges(User.Identity.GetUserId<int>()))
                {
                    Warning("This date might already been registered on another report.", false);
                    return View(model);
                }

                Success("Successfully created new daily report.");

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            return View(model);
        }

        public JsonResult DailyTradingReportList(int offset, int limit, string sort, string order, string dateFrom, string dateTo)
        {
            var queryableList = _reportService.GetAllDailyTrading();
            var allRowCount = queryableList.Count();

            if (!dateFrom.IsNullOrEmpty())
            {
                DateTime date;
                DateTime.TryParseExact(dateFrom, DBCDateFormat.ddMMMyyyy, CultureInfo.CurrentCulture, DateTimeStyles.None, out date);
                if(date != null)
                {
                    queryableList = queryableList.Where(x => x.Date >= date);
                }
            }

            if (!dateTo.IsNullOrEmpty())
            {
                DateTime date;
                DateTime.TryParseExact(dateTo, DBCDateFormat.ddMMMyyyy, CultureInfo.CurrentCulture, DateTimeStyles.None, out date);

                if (date != null)
                {
                    queryableList = queryableList.Where(x => x.Date <= date);
                }
            }

            queryableList = queryableList.PaginateList(x => x.Date, sort, order, offset, limit);

            var rowsResult = queryableList.ToList()
                .Select(x => new DailyTradingListViewModels()
                {
                    Date = x.Date.ToUserLocalDate(User.Identity.GetUserTimeZone()),
                    EURJPY = x.EURJPY,
                    EURNZD = x.EURNZD,
                    EURUSD = x.EURUSD,
                    USDCAD = x.USDCAD,
                    USDSGD = x.USDSGD,
                    GBPUSD = x.GBPUSD,
                    Profit = x.Profit,
                    ActionLinks = new List<ActionLink>()
                    {
                        new ActionLink() {
                            Name = "Edit",
                            Url = Url.RouteUrl(new {
                                id = x.TradeId,
                                controller = "DailyTrading",
                                action = "Edit"
                            })
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

        [Authorize(Roles = DBCRole.Admin)]
        public ActionResult Edit(int id)
        {
            var dailyTrading = _reportService.GetDailyTrading(id);

            AutoMapper.Mapper.Initialize(cfg => cfg.CreateMap<DailyTrading, DailyTradingEditViewModels>());

            var model = AutoMapper.Mapper.Map<DailyTrading, DailyTradingEditViewModels>(dailyTrading);
            model.Date = dailyTrading.Date.ConvertToDateString(User.Identity.GetUserTimeZone());

            return View(model);
        }

        [Authorize(Roles = DBCRole.Admin)]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit(DailyTradingEditViewModels model, int id)
        {
            try
            {
                //get original record
                var dailyTrading = _reportService.GetDailyTrading(id);

                if (!model.Date.IsValidStringDate())
                {
                    ModelState.AddModelError(model.Date, "Invalid date input.");
                }

                if (ModelState.IsValid)
                {
                    var totalProfit = model.EURJPY + model.EURNZD + model.EURUSD
                        + model.GBPUSD + model.USDCAD + model.USDSGD;

                    dailyTrading.EURJPY = model.EURJPY;
                    dailyTrading.EURNZD = model.EURNZD;
                    dailyTrading.EURUSD = model.EURUSD;
                    dailyTrading.GBPUSD = model.GBPUSD;
                    dailyTrading.USDCAD = model.USDCAD;
                    dailyTrading.USDSGD = model.USDSGD;
                    dailyTrading.Profit = totalProfit;
                    dailyTrading.ModifiedTimestamp = DateTime.UtcNow;

                    _reportService.UpdateDailyTrading(dailyTrading);
                }

                _reportService.SaveChanges(User.Identity.GetUserId<int>());

                Success("Successfully modified new daily report.");

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            return View(model);
        }
    }
}