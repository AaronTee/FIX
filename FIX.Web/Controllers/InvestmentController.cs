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

namespace FIX.Web.Controllers
{
    [Authorize]
    public class InvestmentController : BaseController
    {
        IInvestmentService _investmentService;

        public InvestmentController(IInvestmentService investmentService)
        {
            _investmentService = investmentService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            InvestmentCreateModel model = new InvestmentCreateModel();

            return View();
        }

        [HttpPost]
        public ActionResult Create(InvestmentCreateModel model)
        {
            var package = _investmentService.GetEntitledPackage(model.Amount);

            UserPackage up = new UserPackage
            {
                PackageId = package.PackageId,
                UserId = User.Identity.GetUserId<int>(),
                StatusId = (int)EStatus.Active,
                TotalAmount = model.Amount,
                Date = DateTime.UtcNow,
                CreatedTimestamp = DateTime.UtcNow,
                UserPackageDetail = new Lazy<List<UserPackageDetail>>(() =>
                {
                    List<UserPackageDetail> list = new List<UserPackageDetail>();
                    for (int i = 0; i < DBCPackageLifetime.Month; i++)
                    {
                        list.Add(new UserPackageDetail
                        {
                            Date = DateTime.UtcNow.AddMonths(i + 1),
                            Amount = model.Amount * package.Rate,
                            StatusId = (int)EStatus.Pending,
                        });
                    }

                    return list;
                }).Value
            };

            _investmentService.InsertUserPackage(up);

            _investmentService.SaveChange(User.Identity.GetUserId<int>());

            Success("Added new package " + package.Description + ".", true, true);

            return RedirectToAction("Index");
        }

        public JsonResult UserPackageList(string order)
        {
            var tz = User.Identity.GetUserTimeZone();
            var data = _investmentService.GetAllUserPackage(User.Identity.GetUserId<int>()).ToList().Select(x => new
            {
                UserPackageId = x.UserPackageId,
                Package = x.Package.Description,
                StartDate = x.Date.ToUserLocalDate(tz),
                EndDate = x.Date.AddMonths(DBCPackageLifetime.Month).ToUserLocalDate(tz),
                ReturnRate = x.Package.Rate,
                Status = x.Status.Description
            });

            var model = new
            {
                total = data.Count(),
                rows = data
            };

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UserPackageDetailList(int? userPackageId, string order)
        {
            if (userPackageId != null)
            {
                var tz = User.Identity.GetUserTimeZone();
                var data = _investmentService.GetAllUserPackageDetail(userPackageId.Value).ToList().Select(x => new
                {
                    UserPackageId = x.UserPackageId,
                    Date = x.Date.ToUserLocalDate(tz),
                    ReturnRate = x.UserPackage.Package.Rate,
                    Amount = x.Amount,
                    Status = x.Status.Description
                });

                var model = new
                {
                    total = data.Count(),
                    rows = data
                };

                return Json(model, JsonRequestBehavior.AllowGet);
            }

            return Json(new HttpStatusCodeResult(HttpStatusCode.NotFound), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PackageList(string order)
        {
            var list = _investmentService.GetAllPackage().ToList().Select(x => new
            {
                Description = x.Description,
                Rate = x.Rate,
                Threshold = x.Threshold
            });

            var model = new
            {
                total = list.Count(),
                rows = list
            };

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EntitledPackage(decimal? amount, string order)
        {
            if (amount != null)
            {
                var package = _investmentService.GetEntitledPackage(amount.Value);
                return Json(new
                {
                    Rate = (package.Rate * 100) + "%",
                    Description = package.Description
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                Rate = "-",
                Description = "-",
            }, JsonRequestBehavior.AllowGet);
        }
    }
}