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
    public class MyInvestmentController : BaseController
    {
        IUserService _userService;

        public MyInvestmentController(IUserService userService) : base(userService)
        {
            _userService = userService;
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}