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
using SyntrinoWeb.Attributes;

namespace FIX.Web.Controllers
{
    [Authorize]
    [IdentityAuthorize]
    public class InvestmentController : BaseController
    {
        IInvestmentService _investmentService;
        IUserService _userService;

        public InvestmentController(IInvestmentService investmentService, IUserService userService)
        {
            _investmentService = investmentService;
            _userService = userService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = DBCRole.Admin)]
        public ActionResult Manage()
        {
            return View();
        }

        [Authorize(Roles = DBCRole.Admin)]
        public ActionResult Void(int? UserPackageId)
        {
            EJState result = EJState.Unknown;
            try
            {
                var userPackage = _investmentService.GetUserPackage(UserPackageId.Value);
                userPackage.StatusId = (int)EStatus.Void;
                userPackage.ModifiedBy = User.Identity.Name;
                userPackage.ModifiedTimestamp = DateTime.UtcNow;

                _investmentService.UpdateUserPackage(userPackage);
                _investmentService.SaveChange(User.Identity.GetUserId<int>());

                result = EJState.Success;

            }catch(Exception ex)
            {
                Log.Error(ex.Message, ex);
                result = EJState.Failed;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = DBCRole.Admin)]
        public ActionResult Approve(int? UserPackageId)
        {
            EJState result = EJState.Unknown;
            try
            {
                var userPackage = _investmentService.GetUserPackage(UserPackageId.Value);
                var package = _investmentService.GetEntitledPackage(userPackage.TotalAmount);

                //Activate User Package
                userPackage.ApprovedBy = User.Identity.Name;
                userPackage.EffectiveDate = DateTime.UtcNow;
                userPackage.StatusId = (int)EStatus.Active;
                userPackage.ModifiedBy = User.Identity.Name;
                userPackage.ModifiedTimestamp = DateTime.UtcNow;
                _investmentService.UpdateUserPackage(userPackage);

                //Create Return Interest Records
                List<ReturnInterest> RIList = new List<ReturnInterest>();
                for (int i = 0; i < DBCPackageLifetime.Month; i++)
                {
                    ReturnInterest ri = new ReturnInterest
                    {
                        EffectiveDate = DateTime.UtcNow.AddMonths(i + 1).AddDays(1),
                        Amount = userPackage.TotalAmount * package.Rate,
                        StatusId = (int)EStatus.Pending,
                    };

                    _investmentService.InsertReturnInvestment(ri);
                }

                //Create Matching Bonus Records
                List<MatchingBonus> MBList = new List<MatchingBonus>();
                User referralUser = _userService.GetReferralBy(User.Identity.GetUserId<int>());
                var Rate = MatchingBonusSetting.StartingRate;
                for (int i = 1; i <= DBConstant.MatchingBonusSetting.Level; i++)
                {
                    //Find upper level user
                    if (referralUser == null || referralUser.UserProfile.Role.Description == DBConstant.DBCRole.Admin || referralUser.UserProfile.Role.Description == DBConstant.DBCRole.SuperAdmin) break;
                    for (int y = 0; y < DBCPackageLifetime.Month; y++)
                    {
                        MatchingBonus mb = new MatchingBonus
                        {
                            ReferralId = referralUser.UserId,
                            UserId = User.Identity.GetUserId<int>(),
                            Generation = i,
                            Rate = Rate,
                            ReturnDate = DateTime.UtcNow.AddMonths(y + 1).AddDays(1),
                            BonusAmount = userPackage.TotalAmount * package.Rate * Rate,
                            StatusId = (int)EStatus.Pending
                        };

                        _investmentService.InsertMatchingBonus(mb);
                    }
                    referralUser = _userService.GetReferralBy(referralUser.UserId);
                    Rate -= MatchingBonusSetting.DecreaseRate;
                }

                _investmentService.SaveChange(User.Identity.GetUserId<int>());
                result = EJState.Success;
            }
            catch(Exception ex)
            {
                Log.Error(ex.Message, ex);
                result = EJState.Failed;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
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
                TotalAmount = model.Amount,
                CreatedTimestamp = DateTime.UtcNow,
                ReceiptBank = model.Bank,
                ReceiptNo = model.ReferenceNo,
                StatusId = (int)EStatus.Pending,
            };

            _investmentService.InsertUserPackage(up);
            _investmentService.SaveChange(User.Identity.GetUserId<int>());

            Success("Added new package " + package.Description + " and pending for approval.");

            return RedirectToAction("Index");
        }

        public JsonResult UserPackageList(string order)
        {
            var tz = User.Identity.GetUserTimeZone();
            var data = _investmentService.GetAllUserPackage(User.Identity.GetUserId<int>()).ToList().Select(x => new
            {
                UserPackageId = x.UserPackageId,
                Package = x.Package.Description,
                StartDate = x.EffectiveDate.Value.ToUserLocalDate(tz),
                EndDate = x.EffectiveDate.Value.AddMonths(DBCPackageLifetime.Month).ToUserLocalDate(tz),
                InvestedAmount = x.TotalAmount,
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
                var data = _investmentService.GetAllReturnInterest(userPackageId.Value).ToList().Select(x => new
                {
                    UserPackageId = x.UserPackageId,
                    Date = x.EffectiveDate.ToUserLocalDate(tz),
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

        [AllowAnonymous]
        public JsonResult PackageList(string order)
        {
            var list = _investmentService.GetAllPackage().ToList().Select(x => new
            {
                Description = x.Description,
                Rate = x.Rate,
                Threshold = x.Threshold
            }).OrderBy(x => x.Threshold);

            var model = new
            {
                total = list.Count(),
                rows = list
            };

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
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