using FIX.Web.Models;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using AutoMapper;
using FIX.Web.Extensions;
using FIX.Service;
using System.Linq;
using static FIX.Service.DBConstant;
using SyntrinoWeb.Attributes;
using System;

namespace FIX.Web.Controllers
{
    [Authorize(Roles = DBCRole.User)]
    [IdentityAuthorize]
    public class HomeController : BaseController
    {
        IUserService _userService;
        IFinancialService _financialService;
        IInvestmentService _investmentService;
        IArticleService _articleService;

        public HomeController(IUserService userService, IFinancialService financialService, IInvestmentService investmentService, IArticleService articleService)
        {
            _userService = userService;
            _financialService = financialService;
            _investmentService = investmentService;
            _articleService = articleService;
        }

        public ActionResult Index()
        {
            try
            {
                HomeViewModels model = new HomeViewModels();

                var user = _userService.GetUserBy(User.Identity.GetUserId<int>());

                var asdasd = User.Identity.GetUserId<int>();

                model.userModel = new UserViewModel()
                {
                    Username = user.Username,
                    Name = user.UserProfile.Name,
                    CreatedTimestamp = user.CreatedTimestamp.Date
                };

                model.WalletBalance = _financialService.GetUserWalletAvailableBalance(user.UserWallet.First().WalletId) ?? decimal.Zero;
                model.ActivePackagesCount = _investmentService.GetAllUserPackage(user.UserId).Where(x => x.StatusId != (int)EStatus.Deactivated && x.StatusId != (int)EStatus.Void).Count();
                model.BonusAmount = _financialService.GetMatchingBonusReceivedAmount(user.UserWallet.First().WalletId) ?? decimal.Zero;
                model.AnnouncementHtml = _articleService.GetPost((int)EPostType.Announcement).Content;

                return View(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return new AccountController(_userService).LogOff();
            }
        }
    }
}