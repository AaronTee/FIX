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
        private IFinancialService _financialService;
        private IDocService _docService;

        // GET: MatchingBonus
        public MatchingBonusController(IUserService userService, IInvestmentService investmentService, IFinancialService financialService, IDocService docService)
        {
            _userService = userService;
            _investmentService = investmentService;
            _financialService = financialService;
            _docService = docService;
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
                    BonusAmount = item.BonusAmount.ToString(),
                    Status = item.Status.Description,
                    ActionTags = new List<ActionTag>
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

        public JsonResult ApproveMatchingBonus(int MatchingBonusId)
        {
            try
            {
                var matchingBonus = _investmentService.GetMatchingBonus(MatchingBonusId);
                matchingBonus.StatusId = (int)EStatus.Approved;

                var docNo = _docService.GetNextDocumentNumber(DBCDocSequence.EDocSequenceId.Matching_Bonus);

                //Add to wallet
                _financialService.TransactWalletCredit(EOperator.ADD, ETransactionType.Matching_Bonus, matchingBonus.BonusAmount, docNo, matchingBonus.UserEntity.UserWallet.First().WalletId);

                _financialService.SaveChange(User.Identity.GetUserId<int>());

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult VoidMatchingBonus(int MatchingBonusId)
        {
            try
            {
                var matchingBonus = _investmentService.GetMatchingBonus(MatchingBonusId);
                matchingBonus.StatusId = (int)EStatus.Void;
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
    }
}