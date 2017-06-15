using FIX.Service.Interface;
using FIX.Web.Models;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using AutoMapper;
using FIX.Core.Data;
using FIX.Web.Extensions;

namespace FIX.Web.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        IUserService _userService;

        public HomeController(IUserService userService, IBaseService baseService) : base(baseService)
        {
            _userService = userService;
        }

        public ActionResult Index()
        {
            HomeViewModels model = new HomeViewModels();

            var userID = _userService.GetUserID(User.Identity.GetUserId());

            User user = _userService.GetUser(userID);

            model.userModel = new UserViewModel()
            {
                Username = user.Username,
                FirstName = user.UserProfile?.FirstName,
                LastName = user.UserProfile?.LastName,
                CreatedTimestamp = user.CreatedTimestamp
            };

            return View(model);
        }

        public ActionResult Contact()
        {
            //May change as per request
            return View();
        }
    }
}