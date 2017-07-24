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

            var user = _userService.GetUserBy(User.Identity.GetUserId<int>());

            var asdasd = User.Identity.GetUserId<int>();

            model.userModel = new UserViewModel()
            {
                Username = user.Username,
                Name = user.UserProfile.Name,
                hasEmailVerified = user.HasEmailVerified,
                hasAcceptedTerms = user.HasAcceptedTerms,
                CreatedTimestamp = user.CreatedTimestamp.Date
            };

            return View(model);
        }
    }
}