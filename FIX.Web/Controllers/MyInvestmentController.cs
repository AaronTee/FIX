using FIX.Web.Models;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using AutoMapper;
using FIX.Web.Extensions;
using FIX.Service;

namespace FIX.Web.Controllers
{
    [Authorize]
    public class MyInvestmentController : BaseController
    {
        IUserService _userService;

        public MyInvestmentController(IUserService userService)
        {
            _userService = userService;
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}