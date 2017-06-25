using FIX.Web.Models;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using AutoMapper;
using FIX.Web.Extensions;
using FIX.Service;

namespace FIX.Web.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        IUserService _userService;

        public HomeController(IUserService userService)
        {
            _userService = userService;
        }

        public ActionResult Index()
        {
            HomeViewModels model = new HomeViewModels();

            var user = _userService.GetUserBy(User.Identity.GetUserId());

            model.userModel = new UserViewModel()
            {
                Username = user.Username,
                FirstName = user.UserProfile?.FirstName,
                LastName = user.UserProfile?.LastName,
                hasEmailVerified = user.HasEmailVerified,
                hasAcceptedTerms = user.HasAcceptedTerms,
                CreatedTimestamp = user.CreatedTimestamp.Date
            };

            if (!model.userModel.hasEmailVerified)
            {
                ViewBag.Url = ""

                return View("RequiredAction");
            }

            return View(model);
        }

        public ActionResult Contact()
        {
            //May change as per request
            return View();
        }
    }
}